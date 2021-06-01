using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Core.Internal;
using Telegram.Bot.AspNetPipeline.Exceptions;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot.UpdateContextFastSearching;
using Telegram.Bot.AspNetPipeline.Extensions.Logging;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot
{
    public class BotExtSingleton : IBotExtSingleton
    {
        readonly ILogger _logger;

        readonly IUpdateContextSearchBag _searchBag;

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
        public async Task<Message> ReadMessageAsync(
            UpdateContext updateContext, 
            ReadCallbackFromType fromType,
            CancellationToken cancellationToken
            )
        {
            UpdateValidatorDelegate updateValidator = async (newCtx, origCtx) =>
             {
                 return CheckFromType(newCtx, origCtx, fromType);
             };
            return await ReadMessageAsync(updateContext, updateValidator, cancellationToken);
        }

        /// <summary>
        /// </summary>
        /// <param name="updateContext">Current command context. Needed to find TaskCompletionSource of current command.</param>
        /// <param name="updateValidator">User delegate to check if Update from current context is fits.
        /// If true - current Update passed to callback result, else - will be processed by other controller actions with lower priority.</param>
        public async Task<Message> ReadMessageAsync(
            UpdateContext updateContext,
            UpdateValidatorDelegate updateValidator,
            CancellationToken cancellationToken
            )
        {
            var res = await ReadUpdateAsync(updateContext, updateValidator, cancellationToken);
            return res.Message;
        }

        public async Task<Update> ReadUpdateAsync(
            UpdateContext updateContext,
            UpdateValidatorDelegate updateValidator,
            CancellationToken cancellationToken
            )
        {
            var taskCompletionSource = new TaskCompletionSource<Update>(
                TaskContinuationOptions.RunContinuationsAsynchronously
                );

            var searchData = Add(updateContext, taskCompletionSource, updateValidator);

            //OnCanceled.
            cancellationToken.Register(() =>
            {
                SetCancelled(searchData);
            });

            var resUpdate = await taskCompletionSource.Task;
            return resUpdate;
        }

        public async Task OnUpdateInvoke(UpdateContext newCtx, Func<Task> next)
        {
            _logger.LogTrace(
                "Checking read-callback for '{0}'.",
                newCtx
                );
            var searchDataNullable = _searchBag.TryFind(newCtx.ChatId.Identifier, newCtx.Bot.BotId);
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
                        if (updateValidatorRes != UpdateValidatorResult.Valid)
                            break;
                        updateValidatorRes = await globalValidator.Invoke(newCtx, origCtx);
                    }
                }
                catch (Exception ex)
                {
                    SetException(
                        searchData,
                        new TelegramAspException("Exception in update validator delegate.", ex)
                        );

                    return;
                }

                if (updateValidatorRes == UpdateValidatorResult.ContinueWaiting)
                {
                    await next();
                    return;
                }
                if (updateValidatorRes == UpdateValidatorResult.AbortWaiter)
                {

                    SetCancelled(searchData);
                    await next();
                    return;
                }

                //Force exit only if result valid.
                SetResult(searchData, newCtx);


            }
            else
            {
                await next();
            }
        }

        UpdateContextSearchData Add(UpdateContext updateContext, TaskCompletionSource<Update> taskCompletionSource, UpdateValidatorDelegate updateValidator)
        {
            var chatId = updateContext.ChatId.Identifier;
            var botId = updateContext.Bot.BotId;
            var prevData = _searchBag.TryRemove(chatId, botId);
            if (prevData != null)
            {
                _logger.LogInformation(
                    "UpdateContext '{0}' cancelled while adding new context with same chatId and botId.",
                    prevData.Value.CurrentUpdateContext
                    );
                SetCancelled(prevData.Value);
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
            return sd;
        }

        UpdateValidatorResult CheckFromType(UpdateContext newCtx, UpdateContext origCtx, ReadCallbackFromType fromType)
        {
            var res = ReadCallbackFromDefaultValidator.Check(newCtx, origCtx, fromType);
            return res ? UpdateValidatorResult.Valid : UpdateValidatorResult.ContinueWaiting;
        }

        void SetCancelled(UpdateContextSearchData searchData)
        {
            var taskCompletionSource = searchData.TaskCompletionSource;
            var origCtx = searchData.CurrentUpdateContext;
            _searchBag.TryRemove(origCtx.ChatId.Identifier, origCtx.Bot.BotId);

            //Task will be continued with current thread context of readed UpdateContext object.
            //So we don`t need to start it with IExecutionManager.
            var t = taskCompletionSource.Task;
            if (t.IsCanceled || t.IsCompleted || t.IsFaulted)
                return;
            taskCompletionSource.SetCanceled();
        }

        void SetResult(UpdateContextSearchData searchData, UpdateContext newCtx)
        {
            var taskCompletionSource = searchData.TaskCompletionSource;
            var origCtx = searchData.CurrentUpdateContext;
            _searchBag.TryRemove(origCtx.ChatId.Identifier, origCtx.Bot.BotId);
            newCtx.ForceExit();

            //Task will be continued with current thread context of readed UpdateContext object.
            //So we don`t need to start it with IExecutionManager.
            var t = taskCompletionSource.Task;
            if (t.IsCanceled || t.IsCompleted || t.IsFaulted)
                return;
            taskCompletionSource.SetResult(newCtx.Update);
        }

        void SetException(UpdateContextSearchData searchData, Exception ex)
        {
            var taskCompletionSource = searchData.TaskCompletionSource;
            var origCtx = searchData.CurrentUpdateContext;
            _searchBag.TryRemove(origCtx.ChatId.Identifier, origCtx.Bot.BotId);

            //Task will be continued with current thread context of readed UpdateContext object.
            //So we don`t need to start it with IExecutionManager.
            var t = taskCompletionSource.Task;
            if (t.IsCanceled || t.IsCompleted || t.IsFaulted)
                return;
            taskCompletionSource.SetException(ex);
        }
    }
}
