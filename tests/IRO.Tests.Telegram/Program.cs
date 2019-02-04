using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using IRO.Tests.Telegram.Controllers;
using IRO.Tests.Telegram.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Mvc.Builder;
using Telegram.Bot.Types.Enums;

namespace IRO.Tests.Telegram
{

    class Program
    {
        static void Main(string[] args)
        {
            var logger=new NLogLoggerFactory(new NLogProviderOptions()
            {
                //IncludeScopes = true,
                //CaptureMessageProperties = true
            }).CreateLogger("MyLog");
            while (true)
            {
                logger.LogError("111");
                NLog.NestedDiagnosticsContext.Push("FOK U");
                using (logger.BeginScope("Scooooope"))
                {
                    logger.LogError("3333");
                };
                Console.ReadLine();
            }

            var token = BotStaticTestsHelpers.GetToken();
            var bot = new TelegramBotClient(token);
            var botHandler = new BotHandler(bot);

            var botTest = new BotTests_ReadMiddleware();
            botTest.Run(botHandler, true);

            while (true)
            {
                Console.ReadLine();
            }
        }

    }
}
