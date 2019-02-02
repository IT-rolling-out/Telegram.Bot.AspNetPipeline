using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using IRO.Tests.Telegram.Controllers;
using IRO.Tests.Telegram.Services;
using Microsoft.Extensions.DependencyInjection;
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
            var token = BotStaticTestsHelpers.GetToken();
            var bot = new TelegramBotClient(token);
            var botHandler = new BotHandler(bot);

            BotTests_ReadMiddleware.Run(botHandler);

            while (true)
            {
                Console.ReadLine();
            }
        }

    }
}
