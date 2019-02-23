using IRO.Tests.Telegram.Services;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Extensions.DevExceptionMessage;
using Telegram.Bot.AspNetPipeline.Extensions.ExceptionHandling;
using Telegram.Bot.AspNetPipeline.Mvc.Builder;

namespace IRO.Tests.Telegram
{
    class BotTests_Mvc
    {
        public void Run(BotManager botManager, bool isDebug)
        {
            botManager.ConfigureServices((servicesWrap) =>
            {
                LoggerStarter.InitLogger(servicesWrap);
                servicesWrap.AddMvc(new MvcOptions()
                {
                    //ConfigureBotExtWithMvc = false
                });

                servicesWrap.Services.AddScoped<ISomeScopedService, SomeScopedService>();
            });

            botManager.ConfigureBuilder((builder) =>
            {
                if (isDebug)
                    builder.UseDevEceptionMessage();
                builder.UseExceptionHandler(async (ctx, ex) =>
                {
                    //Throw exception if false. False mean 'not handled'.
                    return false;
                });

                builder.UseMvc(mvcBuilder =>
                {
                    mvcBuilder.MapRouteAction(async (actionCtx) =>
                    {
                        await actionCtx.UpdateContext.SendTextMessageAsync("Mvc works.");
                    }, template: "/mvc");
                });
            });
            botManager.Start();
        }
    }
}
