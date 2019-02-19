using IRO.Tests.Telegram.Services;
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
        public void Run(BotHandler botHandler, bool isDebug)
        {
            botHandler.ConfigureServices((servicesWrap) =>
            {
                LoggerStarter.InitLogger(servicesWrap);
                servicesWrap.AddMvc(new MvcOptions()
                {
                    //ConfigureBotExtWithMvc = false
                });
            });

            botHandler.ConfigureBuilder((builder) =>
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
            botHandler.Start();
        }
    }
}
