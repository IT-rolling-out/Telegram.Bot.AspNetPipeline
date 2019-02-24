using System;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Core;

namespace Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot
{
    internal class ImprovedBotMiddleware : IMiddleware
    {
        readonly IBotExtSingleton _botExtSingleton;

        public ImprovedBotMiddleware(IBotExtSingleton botExtSingleton)
        {
            _botExtSingleton = botExtSingleton;
        }

        public async Task Invoke(UpdateContext ctx, Func<Task> next)
        {
            await _botExtSingleton.OnUpdateInvoke(ctx, next);
        }
    }
}
