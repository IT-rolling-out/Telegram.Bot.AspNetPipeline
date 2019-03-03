using System;
using Telegram.Bot;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Mvc.Builder;
using Telegram.Bot.AspNetPipeline.Mvc.Extensions;

namespace IRO.Samples.Standalone
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new TelegramBotClient(
                BotTokenResolver.GetToken(),
                new QueuedHttpClient(TimeSpan.FromSeconds(1))
                );
            var botManager = new BotManager(bot);
            botManager.ConfigureServices((servicesWrap) =>
            {
                servicesWrap.AddMvc(new MvcOptions()
                {
                    //Useful for debugging.
                    CheckEqualsRouteInfo = true
                });

                //Logging service example with NLog you can see in IRO.Tests.Telegram.
            });

            botManager.ConfigureBuilder(builder =>
            {
                builder.UseDevEceptionMessage();
                builder.UseOldUpdatesIgnoring();
                builder.UseMvc(mvcBuilder =>
                {
                    //Write /debug to see info about routing.
                    mvcBuilder.UseDebugInfo();

                    mvcBuilder.MapRouteAction(
                        async (actionCtx) =>
                        {
                            await actionCtx.UpdateContext.SendTextMessageAsync("Bot commands works.");
                        },
                        template: "/help"
                        );
                });
            });

            //Default implemention use standart ITelegramBotClient polling.
            //You can add webhooks implemention using Telegram.Bot.AspNetPipeline.WebhookSupport or
            //write your own IUpdatesReceiver.
            botManager.Setup();
            botManager.Start();

            Console.WriteLine("Bot initialized.");
            while (true)
            {
                Console.ReadLine();
            }

            botManager.Dispose();
        }

        static void Main2(string[] args)
        {
            var bot = new TelegramBotClient("<token>");
            var botManager = new BotManager(bot);
            botManager.ConfigureServices((servicesWrap) =>
            {
                servicesWrap.AddMvc();
            });
            botManager.ConfigureBuilder(builder =>
            {
                builder.UseMvc();
            });
            botManager.Start();
        }
    }
}
