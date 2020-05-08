using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Metadata;

namespace IRO.Tests.Telegram.TwoBotsOneServiceProvider.TelegramControllers
{
    public class FirstBotController : BotController
    {
        [BotRoute("/hi")]
        public async Task Hi()
        {
            await SendTextMessageAsync("Hi, i am first bot.");
        }
    }
}
