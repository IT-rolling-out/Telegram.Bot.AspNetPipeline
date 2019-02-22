using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions.DevExceptionMessage;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot;
using Telegram.Bot.AspNetPipeline.Mvc.Builder;
using Telegram.Bot.Types;

namespace IRO.Tests.Telegram
{
    class BotTests_SimplestMvcApp
    {
        public void Run(BotManager botManager, bool isDebug)
        {
            botManager.ConfigureServices((services) =>
            {
                services.AddMvc();
            });

            botManager.ConfigureBuilder((builder) =>
            {
                if (isDebug)
                    builder.UseDevEceptionMessage();
                builder.UseMvc();
            });
            botManager.Start();
        }
    }
}
