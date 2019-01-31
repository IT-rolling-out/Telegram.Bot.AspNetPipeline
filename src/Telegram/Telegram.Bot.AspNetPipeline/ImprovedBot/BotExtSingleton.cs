using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Core.Services;
using Telegram.Bot.AspNetPipeline.Implementations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.AspNetPipeline.Core.ImprovedBot
{
    public class BotExtSingleton : IBotExtSingleton
    {
        readonly IUpdateContextSearchBag _searchBag;

        readonly IExecutionManager _executionManager;

        public BotExtSingleton(IUpdateContextSearchBag searchBag, IExecutionManager executionManager)
        {
            _searchBag = searchBag;
            _executionManager = executionManager;
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
            TaskCompletionSource<Message> taskCompletionSource = new TaskCompletionSource<Message>();
            Add(updateContext, taskCompletionSource, messageValidator);

            //var hiddenContext=HiddenUpdateContext.Resolve(updateContext);
            //hiddenContext.UpdateProcessingAbortedSource.;

            //OnCanceled.
            updateContext.UpdateProcessingAborted.Register(() => { SetCancelled(taskCompletionSource); });


        }

        void Add(UpdateContext updateContext, TaskCompletionSource<Message> taskCompletionSource, Func<Update, bool> messageValidator)
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

        void OnEveryUpdateContext(UpdateContext updateContext)
        {
            var searchDataNullable = _searchBag.TryFind(updateContext.Chat.Id, updateContext.Bot.BotId);
            if (searchDataNullable != null)
            {
                //!When find pending task with read-callback and same context.
                var searchData = searchDataNullable.Value;
                try
                {
                    searchData.MessageValidator?.Invoke();
                }
                catch
                {

                }
                SetResult(origCtxNullable.Value.TaskCompletionSource, updateContext.Message);
            }
        }

        void SetCancelled(TaskCompletionSource<Message> taskCompletionSource)
        {
            //Continue in execution manager.
            _executionManager.ProcessUpdate(async () =>
            {
                taskCompletionSource.SetCanceled();
            });
        }

        void TrySetResult(TaskCompletionSource<Message> taskCompletionSource, Message message)
        {
            //Continue in execution manager.
            _executionManager.ProcessUpdate(async () =>
            {
                taskCompletionSource.TrySetResult(message);
            });
        }
    }
}
