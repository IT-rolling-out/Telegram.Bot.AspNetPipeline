using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot
{
    public interface IBotExtSingleton
    {
        /// <summary>
        /// Validate all callbacks.
        /// </summary>
        IList<UpdateValidatorDelegate> GlobalValidators { get; }

        /// <summary>
        /// </summary>
        /// <param name="updateContext">Current command context.</param>
        /// <param name="fromType">Used to set which members messages must be processed.</param>
        Task<Message> ReadMessageAsync(
            UpdateContext updateContext,
            ReadCallbackFromType fromType = ReadCallbackFromType.CurrentUser
            );

        /// <summary>
        /// </summary>
        /// <param name="updateContext">Current command context.</param>
        /// <param name="updateValidator">User delegate to check if Update from current context is fits.
        /// If true - current Update passed to callback result, else - will be processed by other controller actions with lower priority.</param>
        Task<Message> ReadMessageAsync(
            UpdateContext updateContext,
            UpdateValidatorDelegate updateValidator
            );

        Task<Update> ReadUpdateAsync(UpdateContext updateContext, UpdateValidatorDelegate updateValidator);

        Task OnUpdateInvoke(UpdateContext newContext, Func<Task> next);
    }
}
