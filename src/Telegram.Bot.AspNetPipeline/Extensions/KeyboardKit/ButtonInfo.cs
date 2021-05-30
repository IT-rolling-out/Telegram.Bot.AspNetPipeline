using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.AspNetPipeline.Extensions.KeyboardKit
{
    public class ButtonInfo
    {
        public bool Visible { get; set; } = true;

        public IKeyboardButton Button { get; set; }

        public string Text { get; set; }

        public ButtonClickHandlerDelegate ClickHandler { get; set; }

        public ButtonInfo() { }

        public ButtonInfo(string text, ButtonClickHandlerDelegate clickHandler)
        {
            Text = text;
            ClickHandler = clickHandler;
        }
    }
}