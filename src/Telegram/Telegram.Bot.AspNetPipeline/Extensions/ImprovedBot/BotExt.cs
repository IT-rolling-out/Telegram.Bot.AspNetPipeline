using System;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot
{
    public class BotExt
    {
        public IBotExtSingleton BotExtSingleton { get; }

        readonly UpdateContext _updateContext;

        public BotExt(IBotExtSingleton botExtSingleton, UpdateContext updateContext)
        {
            BotExtSingleton = botExtSingleton;
            _updateContext = updateContext;
        }

        /// <summary>
        /// </summary>
        /// <param name="fromType">Used to set which members messages must be processed.</param>
        public Task<Message> ReadMessageAsync(ReadCallbackFromType fromType = ReadCallbackFromType.CurrentUser)
        {
            return BotExtSingleton.ReadMessageAsync(_updateContext, fromType);
        }

        /// <summary>
        /// </summary>
       /// <param name="updateValidator">User delegate to check if Update from current context is fits.
        /// If true - current Update passed to callback result, else - will be processed by other controller actions with lower priority.</param>
        public Task<Message> ReadMessageAsync(UpdateValidator updateValidator)
        {
            return BotExtSingleton.ReadMessageAsync(_updateContext, updateValidator);
        }
    }
}
