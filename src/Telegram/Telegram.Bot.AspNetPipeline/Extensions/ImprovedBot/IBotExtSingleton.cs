using System;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot
{
    public interface IBotExtSingleton
    {
        /// <summary>
        /// </summary>
        /// <param name="updateContext">Current command context. Needed to find TaskCompletionSource of current command.</param>
        /// <param name="fromType">Used to set which members messages must be processed.</param>
        Task<Message> ReadMessageAsync(
            UpdateContext updateContext,
            ReadCallbackFromType fromType = ReadCallbackFromType.CurrentUser
            );

        /// <summary>
        /// </summary>
        /// <param name="updateContext">Current command context. Needed to find TaskCompletionSource of current command.</param>
        /// <param name="messageValidator">User delegate to check if Update from current context is fits.
        /// If true - current Update passed to callback result, else - will be processed by other controller actions with lower priority.</param>
        Task<Message> ReadMessageAsync(
            UpdateContext updateContext,
            Func<Update, bool> messageValidator
            );

        Task OnUpdateInvoke(UpdateContext newContext, Func<Task> next);
    }
}
