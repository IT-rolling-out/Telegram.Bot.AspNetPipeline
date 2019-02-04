using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot.UpdateContextFastSearching;
using Telegram.Bot.AspNetPipeline.Extensions.Logging;

namespace Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot
{
    public class ImprovedBotMiddleware : IMiddleware
    {
        readonly ILogger _logger;

        readonly IBotExtSingleton _botExtSingleton;

        public ImprovedBotMiddleware(IBotExtSingleton botExtSingleton, ILoggerFactory loggerFactory)
        {
            _botExtSingleton = botExtSingleton;
            _logger=loggerFactory.CreateLogger(GetType());
        }

        public async Task Invoke(UpdateContext ctx, Func<Task> next)
        {
            using (_logger.BeginScope(ctx.GetLoggerScope()))
            {
                _logger.LogError("Hi");
            }
            using (_logger.BeginScope(ctx.GetLoggerScope()))
            {
                _logger.LogError("mark");
            }
            await _botExtSingleton.OnUpdateInvoke(ctx, next);
        }
    }
}
