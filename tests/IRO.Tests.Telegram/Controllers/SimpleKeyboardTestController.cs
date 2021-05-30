using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Metadata;

namespace IRO.Tests.Telegram.Controllers
{
    public class SimpleKeyboardTestController : BotController
    {
        [BotRoute("/show_simple_keyboard")]
        public async Task ShowSimpleKeyboard()
        {
            await SimpleKeyboard.ShowInlineButtons(new[]
            {
                "btn1", "btn2"
            }, deleteKeyboardMsgAfter: true);
            await SendTextMessageAsync("Keyboard shown.");
        }
    }
}