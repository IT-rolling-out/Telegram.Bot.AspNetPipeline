using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace IRO.Telegram.Bot.ProcessingPipeline.Core.BotExt
{
    public interface IBotStatelessExtensions
    {
        /// <summary>
        /// </summary>
        /// <param name="updateContext">Current command context. Needed to find TaskCompletionSource of current command.</param>
        /// <param name="timeout">Default and max value you can set when configure bot.</param>
        Task<Message> ReadMessageAsync(UpdateContext updateContext, TimeSpan? timeout = null);

        /// <summary>
        /// </summary>
        /// <param name="updateContext">Current command context. Needed to find TaskCompletionSource of current command.</param>
        /// <param name="timeout">Default and max value you can set when configure bot.</param>
        Task<Update> ReadUpdateAsync(UpdateContext updateContext, TimeSpan? timeout = null, params UpdateType[] updateTypes);
    }
}
