using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot.UpdateContextFastSearching;

namespace Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot
{
    public static class ImprovedBotBuilderExtensions
    {
        /// <summary>
        /// Note: Current middleware is registered automatically.
        /// </summary>
        public static void UseBotExt(this IPipelineBuilder @this)
        {
            @this.UseMiddlware<ImprovedBotMiddleware>();
        }

        /// <summary>
        /// Note: Current middleware is registered automatically.
        /// </summary>
        public static void AddBotExt(this IServiceCollection @this)
        {
            @this.AddSingleton<ImprovedBotMiddleware>();
            @this.AddSingleton<IUpdateContextSearchBag, UpdateContextSearchBag>();
            @this.AddSingleton<IBotExtSingleton, BotExtSingleton>();
        }
    }
}
