using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.CloudFileStorage;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.CloudFileStorage.BotsProviders;
using Telegram.Bot.CloudFileStorage.Data;

namespace IRO.Tests.Telegram
{
    class Program
    {
        static void Main(string[] args)
        {
            var token = BotTokenResolver.GetToken();
            var bot = new TelegramBotClient(token);
            var botManager = new BotManager(bot);
           
            var botTest = new BotTests_Mvc();
            botTest.Run(botManager, true);
            

            while (true)
            {
                Console.ReadLine();
            }
        }

    }
}
