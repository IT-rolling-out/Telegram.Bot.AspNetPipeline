using System;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Extensions.KeyboardKit
{
    public class ButtonClickHandlerArgs : EventArgs
    {
        public ITelegramBotClient Bot { get; set; }

        public Update Update { get; set; }

        public ButtonInfo ButtonInfo { get; set; }
    }
}