using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using IRO.Tests.Telegram.Controllers;
using IRO.Tests.Telegram.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions.Serialization;
using Telegram.Bot.AspNetPipeline.Mvc.Builder;
using Telegram.Bot.Types.Enums;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace IRO.Tests.Telegram
{
    class Program
    {
        static void Main(string[] args)
        {
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
