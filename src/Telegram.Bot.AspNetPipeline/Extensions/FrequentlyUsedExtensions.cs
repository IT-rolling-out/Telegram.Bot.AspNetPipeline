using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.AspNetPipeline.Extensions
{
    public static class FrequentlyUsedExtensions
    {
        /// <summary>
        /// Max for telegram is 4096 UTF8  characters.
        /// </summary>
        public const int MaxTelegramMessageLength = 4096;

        /// <summary>
        /// Send text message to current chat.
        /// </summary>
        public static async Task<Message> SendTextMessageAsync(
            this UpdateContext @this,
            string text, 
            ParseMode parseMode = ParseMode.Default, 
            bool disableWebPagePreview = false, 
            bool disableNotification = false, 
            int replyToMessageId = 0, 
            IReplyMarkup replyMarkup = null, 
            CancellationToken cancellationToken = default(CancellationToken)
            )
        {
            return await @this.Bot.SendTextMessageAsync(
                @this.ChatId,
                text,
                parseMode,
                disableWebPagePreview,
                disableNotification,
                replyToMessageId,
                replyMarkup,
                cancellationToken
                );
        }

        /// <summary>
        /// Extension, that will split text to parts and send it as few messages, if they are too long.
        /// <para></para>
        /// Return last message.
        /// <para></para>
        /// Careful with markdown brackets, when message is separated - they will throw errors.
        /// </summary>
        public static async Task<Message> SendTextMessageInPartsAsync(
            this ITelegramBotClient @this,
            ChatId chatId,
            string text,
            ParseMode parseMode = ParseMode.Default
            )
        {
            var utfText = text.ToUTF8();
            while (true)
            {
                if (utfText.Length > MaxTelegramMessageLength)
                {
                    string currentMessage = utfText.Remove(MaxTelegramMessageLength);
                    utfText = utfText.Substring(MaxTelegramMessageLength);
                    await @this.SendTextMessageAsync(
                        chatId,
                        currentMessage,
                        parseMode: parseMode
                        );
                }
                else
                {
                    return await @this.SendTextMessageAsync(
                        chatId,
                        utfText,
                        parseMode: parseMode
                        );
                }
            }
        }

        public static string ToUTF8(this string @this)
        {
            byte[] bytes = Encoding.Default.GetBytes(@this);
            var utfStr = Encoding.UTF8.GetString(bytes);
            return utfStr;
        }
    }
}
