using System;
using System.Collections.Generic;
using IRO.Tests.Telegram.TwoBotsOneServiceProvider.TelegramControllers;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Mvc.Builder;
using Telegram.Bot.AspNetPipeline.Mvc.Extensions;

namespace IRO.Tests.Telegram.TwoBotsOneServiceProvider
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var services = new ServiceCollection();
                var bm1 = ConfFirstBot(services);
                var bm2 = ConfSecondBot(services);
                var sp = services.BuildServiceProvider();

                bm1.Setup(sp);
                bm2.Setup(sp);

                bm1.Start();
                bm2.Start();

                Console.WriteLine("Bots initialized.");
                while (true)
                {
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static BotManager ConfFirstBot(IServiceCollection services)
        {
            var bot = new TelegramBotClient(
                BotTokenResolver.GetToken(),
                new QueuedHttpClient(TimeSpan.FromSeconds(1))
            );
            var botManager = new BotManager(bot, services);
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
                    mvcBuilder.Controllers = new List<Type>()
                    {
                        typeof(FirstBotController)
                    };
                    mvcBuilder.UseDebugInfo();
                });
            });
            return botManager;
        }

        static BotManager ConfSecondBot(IServiceCollection services)
        {
            var bot = new TelegramBotClient(
                BotTokenResolver.GetSecondToken(),
                new QueuedHttpClient(TimeSpan.FromSeconds(1))
            );
            var botManager = new BotManager(bot, services);
            botManager.ConfigureBuilder(builder =>
            {
                builder.UseDevEceptionMessage();
                builder.UseOldUpdatesIgnoring();
                builder.UseMvc(mvcBuilder =>
                {
                    mvcBuilder.Controllers = new List<Type>()
                    {
                        typeof(SecondBotController)
                    };
                    mvcBuilder.UseDebugInfo();
                });
            });
            return botManager;
        }
    }
}
