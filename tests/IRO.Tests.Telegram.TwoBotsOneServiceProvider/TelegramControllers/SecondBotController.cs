using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Metadata;

namespace IRO.Tests.Telegram.TwoBotsOneServiceProvider.TelegramControllers
{
    public class SecondBotController : BotController
    {
        [BotRoute("/hi")]
        public async Task Hi()
        {
            await SendTextMessageAsync("Hi, i am second bot.");
        }
    }
}