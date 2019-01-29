using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.AspNetPipeline.Core.ImprovedBot
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
       /// <param name="isUpdateFits">User delegate to check if Update from current context is fits.
        /// If true - current Update passed to callback result, else - will be processed by other controller actions with lower priority.</param>
        public Task<Message> ReadMessageAsync(Func<Update, bool> isUpdateFits)
        {
            return BotExtSingleton.ReadMessageAsync(_updateContext, isUpdateFits);
        }

        /// <summary>
        /// In 99% is enough to use ReadMessageAsync.
        /// </summary>
        /// <param name="isUpdateFits">User delegate to check if Update from current context is fits.
        /// If true - current Update passed to callback result, else - will be processed by other controller actions with lower priority.</param>
        /// <param name="updateTypes">All by default.</param>
        public Task<Update> ReadUpdateAsync(IEnumerable<UpdateType> updateTypes=null, Func<Update, bool> isUpdateFits = null)
        {
            return BotExtSingleton.ReadUpdateAsync(_updateContext, updateTypes, isUpdateFits);
        }
    }
}
