using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions.DevExceptionMessage;
using Telegram.Bot.AspNetPipeline.Extensions.ExceptionHandler;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot;
using Telegram.Bot.AspNetPipeline.Mvc.Builder;
using Telegram.Bot.Types;

namespace IRO.Tests.Telegram
{
    static class BotTests_Mvc
    {
        public static void Run(BotHandler botHandler)
        {
            botHandler.ConfigureServices((services) =>
            {
                var mvc = services.AddMvc(new AddMvcOptions());
                //mvc.Controllers.Add(...);
            });

            botHandler.ConfigureBuilder((builder) =>
            {
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
                        await actionCtx.Features.StartAnotherAction("Help");
                    }, template:"/info");
                });
            });
            botHandler.Start();
        }
    }
}
