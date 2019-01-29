using Telegram.Bot.AspNetPipeline.Core.Builder;

namespace Telegram.Bot.AspNetPipeline.Core.ImprovedBot
{
    internal static class ImprovedBotPipelineBuilderExtensions
    {
        public static void UseBotExt(this IPipelineBuilder @this)
        {
            @this.UseMiddlware(new ImprovedBotMiddleware());
        }
    }
}
