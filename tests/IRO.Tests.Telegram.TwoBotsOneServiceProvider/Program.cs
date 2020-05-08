using System;
using System.Collections.Generic;
using IRO.Tests.Telegram.TwoBotsOneServiceProvider.Services;
using IRO.Tests.Telegram.TwoBotsOneServiceProvider.TelegramControllers;
using IRO.Tests.Telegram.TwoBotsOneServiceProvider.TelegramControllers.FirstBotControllers;
using IRO.Tests.Telegram.TwoBotsOneServiceProvider.TelegramControllers.SecondBotControllers;
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
            //In this project we use one service collection for two bots.
            //It's not recommended, because bot's will not be totally isolated from each other,
            //But it useful when you have many bot's in one project and want to share singletons between them.
            //Telegram.Bot.AspNetPipeline redesigned to allow user use same service provider for many bots.
            try
            {
                var services = new ServiceCollection();
                services.AddSingleton<CommonService>();
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
                throw;
            }
        }

        static BotManager ConfFirstBot(IServiceCollection services)
        {
            var bot = new TelegramBotClient(
                BotTokenResolver.GetToken(),
                new QueuedHttpClient(TimeSpan.FromMilliseconds(50))
            );
            var botManager = new BotManager(bot, services);
            botManager.ConfigureServices((servicesWrap) =>
            {
                servicesWrap.AddMvc();
            });
            botManager.ConfigureBuilder(builder =>
            {
                builder.UseDevEceptionMessage();
                builder.UseOldUpdatesIgnoring();
                builder.UseMvc(mvcBuilder =>
                {
                    //mvcBuilder.Controllers = new List<Type>()
                    //{
                    //    typeof(FirstBotController)
                    //};
                    mvcBuilder.RemoveControllersByNamespace(
                        "IRO.Tests.Telegram.TwoBotsOneServiceProvider.TelegramControllers.SecondBotControllers"
                    );
                    mvcBuilder.UseDebugInfo();
                });
            });
            return botManager;
        }

        static BotManager ConfSecondBot(IServiceCollection services)
        {
            var bot = new TelegramBotClient(
                BotTokenResolver.GetSecondToken(),
                new QueuedHttpClient(TimeSpan.FromMilliseconds(50))
            );
            var botManager = new BotManager(bot, services);
            botManager.ConfigureServices((servicesWrap) =>
            {
                servicesWrap.AddMvc();
            });
            botManager.ConfigureBuilder(builder =>
            {
                builder.UseDevEceptionMessage();
                builder.UseOldUpdatesIgnoring();
                builder.UseMvc(mvcBuilder =>
                {
                    //mvcBuilder.Controllers = new List<Type>()
                    //{
                    //    typeof(SecondBotController)
                    //};
                    mvcBuilder.RemoveControllersByNamespace(
                        "IRO.Tests.Telegram.TwoBotsOneServiceProvider.TelegramControllers.FirstBotControllers"
                    );
                    mvcBuilder.UseDebugInfo();
                });
            });
            return botManager;
        }
    }
}
