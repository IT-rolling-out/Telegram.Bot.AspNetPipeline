using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.AspNetPipeline.Core.ImprovedBot
{
    public class BotExtSingleton:IBotExtSingleton
    {
        /// <summary>
        /// </summary>
        /// <param name="updateContext">Current command context. Needed to find TaskCompletionSource of current command.</param>
        /// <param name="fromType">Used to set which members messages must be processed.</param>
        public async Task<Message> ReadMessageAsync(UpdateContext updateContext, ReadCallbackFromType fromType = ReadCallbackFromType.CurrentUser)
        {
            await ReadUpdateAsync(
               updateContext,
               new UpdateType[] { UpdateType.Message }
               );
        }

        /// <summary>
        /// </summary>
        /// <param name="updateContext">Current command context. Needed to find TaskCompletionSource of current command.</param>
        /// <param name="isUpdateFits">User delegate to check if Update from current context is fits.
        /// If true - current Update passed to callback result, else - will be processed by other controller actions with lower priority.</param>
        public async Task<Message> ReadMessageAsync(UpdateContext updateContext, Func<Update, bool> isUpdateFits)
        {
            await ReadUpdateAsync(
                updateContext,
                new UpdateType[] { UpdateType.Message }
                );
        }

        /// <summary>
        /// In 99% is enough to use ReadMessageAsync.
        /// </summary>
        /// <param name="updateContext">Current command context. Needed to find TaskCompletionSource of current command.</param>
        /// <param name="isUpdateFits">User delegate to check if Update from current context is fits.
        /// If true - current Update passed to callback result, else - will be processed by other controller actions with lower priority.</param>
        /// <param name="updateTypes">All by default.</param>
        public Task<Update> ReadUpdateAsync(UpdateContext updateContext, IEnumerable<UpdateType> updateTypes = null, Func<Update, bool> isUpdateFits = null)
        {
            throw new NotImplementedException();
        }
    }
}
