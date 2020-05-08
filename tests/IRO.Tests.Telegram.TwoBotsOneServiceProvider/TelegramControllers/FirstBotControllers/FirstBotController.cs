using System.Threading.Tasks;
using IRO.Tests.Telegram.TwoBotsOneServiceProvider.Services;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Metadata;

namespace IRO.Tests.Telegram.TwoBotsOneServiceProvider.TelegramControllers.FirstBotControllers
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
