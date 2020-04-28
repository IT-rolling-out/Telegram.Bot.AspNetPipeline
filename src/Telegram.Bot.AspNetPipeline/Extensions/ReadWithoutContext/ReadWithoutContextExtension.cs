using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.AspNetPipeline.Extensions.ReadWithoutContext
{
    public static class ReadWithoutContextExtension
    {
        /// <summary>
        /// This extension only use only <see cref="ITelegramBotClient"/>, no middleware, so it can't prevent handling by other middlewares in your pipeline.
        /// </summary>
        public static async Task<Message> ReadMessageAsync(
            this ITelegramBotClient bot,
            ChatId chatId,
            NoContextReadCallbackFromType validateType = NoContextReadCallbackFromType.AnyUser
        )
        {
            var tcs = new TaskCompletionSource<Message>(TaskCreationOptions.RunContinuationsAsynchronously);
            EventHandler<Args.MessageEventArgs> ev = null;
            ev = (s, e) =>
            {
                var isValid = ValidateMessage(bot, chatId, e.Message, validateType);
                if (isValid)
                {
                    bot.OnMessage -= ev;
                    tcs.TrySetResult(e.Message);
                }
            };
            bot.OnMessage += ev;
            return await tcs.Task;
        }

        /// <summary>
        /// This extension only use only <see cref="ITelegramBotClient"/>, no middleware, so it can't prevent handling by other middlewares in your pipeline.
        /// </summary>
        public static async Task<Update> ReadUpdateAsync(
            this ITelegramBotClient bot,
            ChatId chatId,
            UpdateValidatorDelegate validatorDelegate = null
        )
        {
            var tcs = new TaskCompletionSource<Update>(TaskCreationOptions.RunContinuationsAsynchronously);
            EventHandler<Args.UpdateEventArgs> ev = null;
            ev = (s, e) =>
            {
                var upd = e.Update;
                if (upd.ExtractChatId().Identifier != chatId.Identifier)
                    return;

                var isValid = validatorDelegate?.Invoke(chatId, e.Update) ?? true;
                if (isValid)
                {
                    bot.OnUpdate -= ev;
                    tcs.TrySetResult(e.Update);
                }
            };
            bot.OnUpdate += ev;
            return await tcs.Task;
        }

        static bool ValidateMessage(ITelegramBotClient bot, ChatId chatId, Message newMsg, NoContextReadCallbackFromType validateType)
        {
            try
            {
                if (newMsg.Chat.Id != chatId.Identifier)
                    return false;

                if (validateType == NoContextReadCallbackFromType.AnyUserReply)
                {
                    if (newMsg.ReplyToMessage?.From.Id != bot.BotId)
                        return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
