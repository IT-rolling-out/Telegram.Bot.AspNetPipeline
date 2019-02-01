using System;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot.UpdateContextFastSearching;

namespace Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot
{
    public class ImprovedBotMiddleware : IMiddleware
    {
        BotExtSingleton _botExtSingleton;

        public ImprovedBotMiddleware(IUpdateContextSearchBag searchBag)
        {
            _botExtSingleton = new BotExtSingleton(searchBag);
        }

        public async Task Invoke(UpdateContext ctx, Func<Task> next)
        {
            await _botExtSingleton.OnUpdateContext(ctx);
        }
    }
}
