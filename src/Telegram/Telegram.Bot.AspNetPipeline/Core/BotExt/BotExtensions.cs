using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.AspNetPipeline.Core.BotExt
{
    public class BotExtensions
    {
        readonly IBotStatelessExtensions _botStatelessExtensions;

        readonly UpdateContext _updateContext;

        public BotExtensions(IBotStatelessExtensions botStatelessExtensions, UpdateContext updateContext)
        {
            _botStatelessExtensions = botStatelessExtensions;
            _updateContext = updateContext;
        }

        /// <summary>
        /// </summary>
        /// <param name="updateContext">Current command context. Needed to find TaskCompletionSource of current command.</param>
        /// <param name="fromType">Used to set which members messages must be processed.</param>
        public Task<Message> ReadMessageAsync(ReadCallbackFromType fromType = ReadCallbackFromType.CurrentUser)
        {
            return _botStatelessExtensions.ReadMessageAsync(_updateContext, fromType);
        }

        /// <summary>
        /// </summary>
        /// <param name="updateContext">Current command context. Needed to find TaskCompletionSource of current command.</param>
        /// <param name="isUpdateFits">User delegate to check if Update from current context is fits.
        /// If true - current Update passed to callback result, else - will be processed by other controller actions with lower priority.</param>
        public Task<Message> ReadMessageAsync(Func<Update, bool> isUpdateFits)
        {
            return _botStatelessExtensions.ReadMessageAsync(_updateContext, isUpdateFits);
        }

        /// <summary>
        /// In 99% is enough to use ReadMessageAsync.
        /// </summary>
        /// <param name="updateContext">Current command context. Needed to find TaskCompletionSource of current command.</param>
        /// <param name="isUpdateFits">User delegate to check if Update from current context is fits.
        /// If true - current Update passed to callback result, else - will be processed by other controller actions with lower priority.</param>
        /// <param name="updateTypes">All by default.</param>
        public Task<Update> ReadUpdateAsync(UpdateType[] updateTypes=null, Func<Update, bool> isUpdateFits = null)
        {
            return _botStatelessExtensions.ReadUpdateAsync(_updateContext, updateTypes, isUpdateFits);
        }
    }
}
