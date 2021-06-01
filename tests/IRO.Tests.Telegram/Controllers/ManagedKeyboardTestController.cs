using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Extensions.KeyboardKit;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Metadata;

namespace IRO.Tests.Telegram.Controllers
{
    public class ManagedKeyboardTestController : BotController
    {
        [BotRoute("/cancel_test")]
        public async Task ForceExitTest()
        {
            await SendTextMessageAsync("Send me /cancel");
            await BotExt.ReadMessageAsync(ManagedKeyboard.DefaultUpdatesValidator);
            await SendTextMessageAsync("This will not be send.");
        }

        [BotRoute("/inline_keyboard_counter_test")]
        public async Task InlineKeyboardCounterTest()
        {
            await KeyboardCounterTest(true);
        }

        [BotRoute("/reply_keyboard_counter_test")]
        public async Task ReplyKeyboardCounterTest()
        {
            await KeyboardCounterTest(false);
        }

        async Task KeyboardCounterTest(bool isInline)
        {
            int counter = 1;
            ManagedKeyboard.IsInlineKeyboard = isInline;
            var kb = new ButtonInfo[][]
            {
                new ButtonInfo[]
                {
                    new ButtonInfo("Click me!",async (s, a) =>
                    {
                        a.ButtonInfo.Text = $"Number: {counter++}.";
                        await ManagedKeyboard.RefreshButtons();
                        if (counter > 5)
                        {
                            ManagedKeyboard.StopListeningUpdates();
                        }
                    }),
                    new ButtonInfo("Stop",async (s, a) =>
                    {
                        ManagedKeyboard.StopListeningUpdates();
                    })
                }
            };
            ManagedKeyboard.SetButtons(kb);
            await ManagedKeyboard.ShowButtonsWithText("Let's count to 5.");
            await ManagedKeyboard.StartListeningUpdates();
            await ManagedKeyboard.DeleteButtons();
            await SendTextMessageAsync("Test finished.");
        }
    }
}
