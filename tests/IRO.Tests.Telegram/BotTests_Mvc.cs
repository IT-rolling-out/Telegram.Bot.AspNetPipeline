using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions.DevExceptionMessage;
using Telegram.Bot.AspNetPipeline.Extensions.ExceptionHandling;
using Telegram.Bot.AspNetPipeline.Mvc.Builder;

namespace IRO.Tests.Telegram
{
    class BotTests_Mvc
    {
        public void Run(BotHandler botHandler, bool isDebug)
        {
            botHandler.ConfigureServices((services) =>
            {
                services.AddMvc();
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
                        //await actionCtx.Features.StartAnotherAction("Help");
                    }, template: "/info");
                });
            });
            botHandler.Start();
        }
    }
}
