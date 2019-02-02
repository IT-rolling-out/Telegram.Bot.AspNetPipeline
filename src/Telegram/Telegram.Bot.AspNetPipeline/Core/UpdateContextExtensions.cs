using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.AspNetPipeline.Core
{
    public static class UpdateContextExtensions
    {
        public static async Task<Message> SendTextMessageAsync(this UpdateContext @this, string text, ParseMode parseMode=ParseMode.Default)
        {
            return await @this.Bot.SendTextMessageAsync(
                @this.Chat.Id, 
                text,
                parseMode:parseMode
                );
        }
    }
}
