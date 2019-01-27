using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace IRO.Telegram.Bot.ProcessingPipeline.Core.BotExt
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
        /// <param name="timeout">Default and max value you can set when configure bot.</param>
        public Task<Message> ReadMessageAsync(TimeSpan? timeout = null)
        {
            return _botStatelessExtensions.ReadMessageAsync(_updateContext, timeout);
        }

        /// <summary>
        /// </summary>
        /// <param name="updateContext">Current command context. Needed to find TaskCompletionSource of current command.</param>
        /// <param name="timeout">Default and max value you can set when configure bot.</param>
        public Task<Update> ReadUpdateAsync(TimeSpan? timeout = null, params UpdateType[] updateTypes)
        {
            return _botStatelessExtensions.ReadUpdateAsync(_updateContext, timeout, updateTypes);
        }
    }
}
