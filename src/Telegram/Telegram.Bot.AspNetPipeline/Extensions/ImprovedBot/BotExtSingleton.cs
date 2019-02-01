using System;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot.UpdateContextFastSearching;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot
{
    public class BotExtSingleton : IBotExtSingleton
    {
        readonly IUpdateContextSearchBag _searchBag;

        public BotExtSingleton(IUpdateContextSearchBag searchBag)
        {
            _searchBag = searchBag;
        }

        /// <summary>
        /// </summary>
        /// <param name="updateContext">Current command context. Needed to find TaskCompletionSource of current command.</param>
        /// <param name="fromType">Used to set which members messages must be processed.</param>
        public async Task<Message> ReadMessageAsync(UpdateContext updateContext, ReadCallbackFromType fromType = ReadCallbackFromType.CurrentUser)
        {
            Func<Update, bool> messageValidator = (upd) => CheckFromType(upd, updateContext, fromType);
            return await ReadMessageAsync(updateContext, messageValidator);

        }

        /// <summary>
        /// </summary>
        /// <param name="updateContext">Current command context. Needed to find TaskCompletionSource of current command.</param>
        /// <param name="messageValidator">User delegate to check if Update from current context is fits.
        /// If true - current Update passed to callback result, else - will be processed by other controller actions with lower priority.</param>
        public async Task<Message> ReadMessageAsync(UpdateContext updateContext, Func<Update, bool> messageValidator)
        {
            TaskCompletionSource<Update> taskCompletionSource = new TaskCompletionSource<Update>();
            Add(updateContext, taskCompletionSource, messageValidator);

            //var hiddenContext=HiddenUpdateContext.Resolve(updateContext);
            //hiddenContext.UpdateProcessingAbortedSource.;

            //OnCanceled.
            updateContext.UpdateProcessingAborted.Register(() => { SetCancelled(taskCompletionSource); });

            var resUpdate = await taskCompletionSource.Task;
            return resUpdate.Message;
        }

        void Add(UpdateContext updateContext, TaskCompletionSource<Update> taskCompletionSource, Func<Update, bool> messageValidator)
        {
            var sd = new UpdateContextSearchData
            {
                CurrentUpdateContext = updateContext,
                ChatId = updateContext.Update.Message.Chat.Id,
                BotId = updateContext.Bot.BotId,
                TaskCompletionSource = taskCompletionSource,
                MessageValidator = messageValidator
            };
            _searchBag.Add(sd);
        }

        bool CheckFromType(Update upd, UpdateContext origCtx, ReadCallbackFromType fromType)
        {
            if (fromType == ReadCallbackFromType.CurrentUser)
            {
                return upd.Message.From.Id == origCtx.Message.From.Id;
            }
            else if (fromType == ReadCallbackFromType.CurrentUserReply)
            {
                if (upd.Message.From.Id != origCtx.Message.From.Id)
                    return false;
                if (upd.Message.ReplyToMessage.From.Id != origCtx.Bot.BotId)
                    return false;
                return true;
            }
            else if (fromType == ReadCallbackFromType.AnyUserReply)
            {
                if (upd.Message.ReplyToMessage.From.Id != origCtx.Bot.BotId)
                    return false;
                return true;
            }
            else if (fromType == ReadCallbackFromType.AnyUser)
            {
                //I know that current expression can be removed, but it make code more readable.
                return true;
            }

            return true;
        }

        internal async Task OnUpdateContext(UpdateContext newContext)
        {
            var searchDataNullable = _searchBag.TryFind(newContext.Chat.Id, newContext.Bot.BotId);
            if (searchDataNullable != null)
            {
                //!When find pending task with read-callback and same context.
                var searchData = searchDataNullable.Value;

                bool isUpdateValid = false;
                try
                {
                    isUpdateValid = searchData.MessageValidator?.Invoke(newContext.Update) == true;
                }
                catch(Exception ex)
                {
                    SetException(
                        searchData.TaskCompletionSource,
                        new Exception("Exception in update validator delegate.", ex)
                        );

                    return;
                }

                if (!isUpdateValid)
                    return;

                //Force exit only if result valid.
                newContext.ForceExit();
                SetResult(searchData.TaskCompletionSource, newContext.Update);
            }
        }

        void SetCancelled(TaskCompletionSource<Update> taskCompletionSource)
        {
            //Task will be continued with current thread context of readed UpdateContext object.
            //So we don`t need to start it with IExecutionManager.
            taskCompletionSource.SetCanceled();
        }

        void SetResult(TaskCompletionSource<Update> taskCompletionSource, Update upd)
        {
            //Task will be continued with current thread context of readed UpdateContext object.
            //So we don`t need to start it with IExecutionManager.
            taskCompletionSource.TrySetResult(upd);
        }

        void SetException(TaskCompletionSource<Update> taskCompletionSource, Exception ex)
        {
            //Task will be continued with current thread context of readed UpdateContext object.
            //So we don`t need to start it with IExecutionManager.
            taskCompletionSource.TrySetException(ex);
        }
    }
}
