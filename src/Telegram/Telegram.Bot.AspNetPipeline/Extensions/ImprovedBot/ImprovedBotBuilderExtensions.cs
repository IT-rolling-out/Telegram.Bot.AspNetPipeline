using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Builder;

namespace Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot
{
    public static class ImprovedBotBuilderExtensions
    {
        /// <summary>
        /// Note: Current middleware is registered automatically.
        /// </summary>
        public static void UseBotExt(this IPipelineBuilder @this)
        {
            var md = @this.ServiceProvider.GetService<ImprovedBotMiddleware>();
            @this.UseMiddlware(md);
        }

        /// <summary>
        /// Note: Current middleware is registered automatically.
        /// </summary>
        public static void AddBotExt(this IServiceCollection @this)
        {
            @this.AddSingleton<ImprovedBotMiddleware>();
        }
    }
}
