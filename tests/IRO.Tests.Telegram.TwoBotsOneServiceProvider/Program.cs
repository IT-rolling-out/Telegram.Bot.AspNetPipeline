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
                var bm1 = ConfFirstBot();
                var bm2 = ConfSecondBot();
                var sp = services.BuildServiceProvider();

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

        static BotManager ConfFirstBot()
        {
            var bot = new TelegramBotClient(
                BotTokenResolver.GetToken(),
                new QueuedHttpClient(TimeSpan.FromSeconds(1))
            );
            var botManager = new BotManager(bot);
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
                    mvcBuilder.Controllers = new List<Type>()
                    {
                        typeof(FirstBotController)
                    };
                    mvcBuilder.UseDebugInfo();
                });
            });
            return botManager;
        }

        static BotManager ConfSecondBot()
        {
            var bot = new TelegramBotClient(
                BotTokenResolver.GetSecondToken(),
                new QueuedHttpClient(TimeSpan.FromSeconds(1))
            );
            var botManager = new BotManager(bot);
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
