using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot.UpdateContextFastSearching;

namespace Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot
{
    public class ImprovedBotMiddleware : IMiddleware
    {
        IBotExtSingleton _botExtSingleton;

        public ImprovedBotMiddleware()
        {

        }

        public async Task Invoke(UpdateContext ctx, Func<Task> next)
        {
            _botExtSingleton = _botExtSingleton ?? ctx.Services.GetService<IBotExtSingleton>();
            await _botExtSingleton.OnUpdateInvoke(ctx, next);
        }
    }
}
