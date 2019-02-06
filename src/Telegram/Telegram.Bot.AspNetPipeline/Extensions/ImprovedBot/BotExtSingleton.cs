using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot.UpdateContextFastSearching;
using Telegram.Bot.AspNetPipeline.Extensions.Logging;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot
{
    public class BotExtSingleton : IBotExtSingleton
    {
        readonly ILogger _logger;

        readonly IUpdateContextSearchBag _searchBag;

        /// <summary>
        /// Validate all callbacks. All must return true to validate it.
        /// </summary>
        public IList<UpdateValidator> GlobalValidators { get; } = new List<UpdateValidator>();

        public BotExtSingleton(IUpdateContextSearchBag searchBag, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType());
            _searchBag = searchBag;
        }

        /// <summary>
        /// </summary>
        /// <param name="updateContext">Current command context. Needed to find TaskCompletionSource of current command.</param>
        /// <param name="fromType">Used to set which members messages must be processed.</param>
        public async Task<Message> ReadMessageAsync(UpdateContext updateContext, ReadCallbackFromType fromType = ReadCallbackFromType.CurrentUser)
        {
            UpdateValidator updateValidator = async (upd, origCtx) =>
             {
                 return CheckFromType(upd, origCtx, fromType);
             };
            return await ReadMessageAsync(updateContext, updateValidator);
        }

        /// <summary>
        /// </summary>
        /// <param name="updateContext">Current command context. Needed to find TaskCompletionSource of current command.</param>
        /// <param name="updateValidator">User delegate to check if Update from current context is fits.
        /// If true - current Update passed to callback result, else - will be processed by other controller actions with lower priority.</param>
        public async Task<Message> ReadMessageAsync(UpdateContext updateContext, UpdateValidator updateValidator)
        {
            TaskCompletionSource<Update> taskCompletionSource = new TaskCompletionSource<Update>();
            Add(updateContext, taskCompletionSource, updateValidator);

            //var hiddenContext=HiddenUpdateContext.Resolve(updateContext);
            //hiddenContext.UpdateProcessingAbortedSource.;

            //OnCanceled.
            updateContext.UpdateProcessingAborted.Register(() =>
            {
                _logger.LogInformation("'{0}' will be cancelled.", updateContext);
                SetCancelled(taskCompletionSource);
            });

            var resUpdate = await taskCompletionSource.Task;
            return resUpdate.Message;
        }

        public async Task OnUpdateInvoke(UpdateContext newContext, Func<Task> next)
        {
            _logger.LogTrace(
                "Checking read-callback for '{0}'.",
                newContext
                );
            var searchDataNullable = _searchBag.TryFind(newContext.Chat.Id, newContext.Bot.BotId);
            if (searchDataNullable != null)
            {
                _logger.LogTrace(
                    "Found read-callback for '{0}'.",
                    newContext
                );

                //!When find pending task with read-callback and same context.
                var searchData = searchDataNullable.Value;
                var origCtx = searchData.CurrentUpdateContext;

                bool isUpdateValid = false;
                try
                {
                    if (searchData.UpdateValidator != null)
                    {
                        isUpdateValid = await searchData.UpdateValidator.Invoke(newContext.Update, origCtx);
                    }

                    foreach (var globalValidator in GlobalValidators)
                    {
                        //Break on first false.
                        if (!isUpdateValid)
                            break;
                        isUpdateValid = await globalValidator.Invoke(newContext.Update, origCtx);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        "Exception while validating Update '{0}'.\nOrigUpdateContext: '{1}'.\nNewContext: '{2}'.",
                        ex,
                        origCtx,
                        newContext
                        );
                    SetException(
                        searchData.TaskCompletionSource,
                        new Exception("Exception in update validator delegate.", ex)
                        );

                    return;
                }

                if (!isUpdateValid)
                {
                    _logger.LogTrace(
                        "'{0}' not valid for read-callback of '{1}'.",
                        origCtx,
                        newContext
                        );
                    await next();
                    return;
                }

                //Force exit only if result valid.
                _logger.LogTrace(
                    "'{0}'  valid for read-callback of '{1}'.",
                    origCtx,
                    newContext
                    );
                SetResult(searchData.TaskCompletionSource, newContext.Update);
                _searchBag.TryRemove(newContext.Chat.Id, newContext.Bot.BotId);
                newContext.ForceExit();
            }
            else
            {
                await next();
            }
        }

        void Add(UpdateContext updateContext, TaskCompletionSource<Update> taskCompletionSource, UpdateValidator updateValidator)
        {
            var chatId = updateContext.Update.Message.Chat.Id;
            var botId = updateContext.Bot.BotId;
            var prevData = _searchBag.TryRemove(chatId, botId);
            if (prevData != null)
            {
                _logger.LogInformation(
                    "UpdateContext '{0}' cancelled while adding new context with same chatId and botId.",
                    prevData.Value.CurrentUpdateContext
                    );
                SetCancelled(prevData.Value.TaskCompletionSource);
            }
            var sd = new UpdateContextSearchData
            {
                CurrentUpdateContext = updateContext,
                ChatId = chatId,
                BotId = botId,
                TaskCompletionSource = taskCompletionSource,
                UpdateValidator = updateValidator
            };
            _searchBag.Add(sd);
            _logger.LogTrace(
                "UpdateContext '{0}' added to SearchBag of read-callbacks.",
                updateContext
                );
        }

        bool CheckFromType(Update upd, UpdateContext origCtx, ReadCallbackFromType fromType)
        {
            try
            {
                _logger.LogTrace("Default CheckFromType for {0}.", fromType);
                if (fromType == ReadCallbackFromType.CurrentUser)
                {
                    return upd.Message.From.Id == origCtx.Message.From.Id;
                }
                else if (fromType == ReadCallbackFromType.CurrentUserReply)
                {
                    if (upd.Message.From.Id != origCtx.Message.From.Id)
                        return false;
                    if (upd.Message.ReplyToMessage?.From.Id != origCtx.Bot.BotId)
                        return false;
                    return true;
                }
                else if (fromType == ReadCallbackFromType.AnyUserReply)
                {
                    if (upd.Message.ReplyToMessage?.From.Id != origCtx.Bot.BotId)
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
            catch
            {
                return false;
            }
        }

        void SetCancelled(TaskCompletionSource<Update> taskCompletionSource)
        {
            //Task will be continued with current thread context of readed UpdateContext object.
            //So we don`t need to start it with IExecutionManager.
            var t = taskCompletionSource.Task;
            if (t.IsCanceled || t.IsCompleted || t.IsFaulted)
                return;
            taskCompletionSource.SetCanceled();
        }

        void SetResult(TaskCompletionSource<Update> taskCompletionSource, Update upd)
        {
            //Task will be continued with current thread context of readed UpdateContext object.
            //So we don`t need to start it with IExecutionManager.
            var t = taskCompletionSource.Task;
            if (t.IsCanceled || t.IsCompleted || t.IsFaulted)
                return;
            taskCompletionSource.SetResult(upd);
        }

        void SetException(TaskCompletionSource<Update> taskCompletionSource, Exception ex)
        {
            //Task will be continued with current thread context of readed UpdateContext object.
            //So we don`t need to start it with IExecutionManager.
            var t = taskCompletionSource.Task;
            if (t.IsCanceled || t.IsCompleted || t.IsFaulted)
                return;
            taskCompletionSource.SetException(ex);
        }
    }
}
