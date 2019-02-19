using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Core.Internal;
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
        public IList<UpdateValidatorDelegate> GlobalValidators { get; } = new List<UpdateValidatorDelegate>();

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
            UpdateValidatorDelegate updateValidator = async (newCtx, origCtx) =>
             {
                 return CheckFromType(newCtx, origCtx, fromType);
             };
            return await ReadMessageAsync(updateContext, updateValidator);
        }

        /// <summary>
        /// </summary>
        /// <param name="updateContext">Current command context. Needed to find TaskCompletionSource of current command.</param>
        /// <param name="updateValidator">User delegate to check if Update from current context is fits.
        /// If true - current Update passed to callback result, else - will be processed by other controller actions with lower priority.</param>
        public async Task<Message> ReadMessageAsync(UpdateContext updateContext, UpdateValidatorDelegate updateValidator)
        {
            var taskCompletionSource = new TaskCompletionSource<Update>(
                TaskContinuationOptions.RunContinuationsAsynchronously
                );
            Add(updateContext, taskCompletionSource, updateValidator);

            //OnCanceled.
            updateContext.UpdateProcessingAborted.Register(() =>
            {
                _logger.LogInformation("'{0}' will be cancelled.", updateContext);
                SetCancelled(taskCompletionSource);
            });

            var resUpdate = await taskCompletionSource.Task;
            return resUpdate.Message;
        }

        public async Task OnUpdateInvoke(UpdateContext newCtx, Func<Task> next)
        {
            _logger.LogTrace(
                "Checking read-callback for '{0}'.",
                newCtx
                );
            var searchDataNullable = _searchBag.TryFind(newCtx.Chat.Id, newCtx.Bot.BotId);
            if (searchDataNullable != null)
            {
                _logger.LogTrace(
                    "Found read-callback for '{0}'.",
                    newCtx
                );

                //!When find pending task with read-callback and same context.
                var searchData = searchDataNullable.Value;
                var origCtx = searchData.CurrentUpdateContext;

                UpdateValidatorResult updateValidatorRes = UpdateValidatorResult.ContinueWaiting;
                try
                {
                    if (searchData.UpdateValidator != null)
                    {
                        updateValidatorRes = await searchData.UpdateValidator.Invoke(newCtx, origCtx);
                    }

                    foreach (var globalValidator in GlobalValidators)
                    {
                        //Break on first not valid.
                        if (updateValidatorRes!=UpdateValidatorResult.Valid)
                            break;
                        updateValidatorRes = await globalValidator.Invoke(newCtx, origCtx);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        "Exception while validating Update '{0}'.\nOrigUpdateContext: '{1}'.\nNewContext: '{2}'.",
                        ex,
                        origCtx,
                        newCtx
                        );
                    SetException(
                        searchData.TaskCompletionSource,
                        new Exception("Exception in update validator delegate.", ex)
                        );

                    return;
                }

                _logger.LogTrace(
                    "'{0}'  validation result for read-callback of '{1}' is {2}.",
                    origCtx,
                    newCtx,
                    updateValidatorRes
                    );

                if (updateValidatorRes==UpdateValidatorResult.ContinueWaiting)
                {
                    await next();
                    return;
                }
                if (updateValidatorRes == UpdateValidatorResult.AbortWaiter)
                {
                    _searchBag.TryRemove(origCtx.Chat.Id, origCtx.Bot.BotId);
                    origCtx.HiddenContext().UpdateProcessingAbortedSource.Cancel();
                    await next();
                    return;
                }

                //Force exit only if result valid.

                SetResult(searchData.TaskCompletionSource, newCtx.Update);
                _searchBag.TryRemove(newCtx.Chat.Id, newCtx.Bot.BotId);
                newCtx.ForceExit();
            }
            else
            {
                await next();
            }
        }

        void Add(UpdateContext updateContext, TaskCompletionSource<Update> taskCompletionSource, UpdateValidatorDelegate updateValidator)
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

        UpdateValidatorResult CheckFromType(UpdateContext newCtx, UpdateContext origCtx, ReadCallbackFromType fromType)
        {
            var res = CheckFromType_BoolResult(newCtx, origCtx, fromType);
            return res ? UpdateValidatorResult.Valid : UpdateValidatorResult.ContinueWaiting;
        }

        bool CheckFromType_BoolResult(UpdateContext newCtx, UpdateContext origCtx, ReadCallbackFromType fromType)
        {
            var upd = newCtx.Update;
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
