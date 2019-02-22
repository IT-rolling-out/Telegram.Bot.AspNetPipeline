using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.AspNetPipeline.Extensions
{
    public static class UpdateContextFrequentlyUsedExtensions
    {
        /// <summary>
        /// Send text message to current chat.
        /// </summary>
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
