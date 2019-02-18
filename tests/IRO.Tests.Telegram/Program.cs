using System;
using Telegram.Bot;
using Telegram.Bot.AspNetPipeline.Core;

namespace IRO.Tests.Telegram
{
    class Program
    {
        static void Main(string[] args)
        {
            var token = BotStaticTestsHelpers.GetToken();
            var bot = new TelegramBotClient(token);
            var botHandler = new BotHandler(bot);

            var botTest = new BotTests_Mvc();
            botTest.Run(botHandler, true);

            while (true)
            {
                Console.ReadLine();
            }
        }

    }
}
