using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Extensions.ReadWithoutContext
{
    public static class ReadWithoutContextExtension
    {
        /// <summary>
        /// This extension only use only <see cref="ITelegramBotClient"/>, no middleware, so it can't prevent handling by other middlewares in your pipeline.
        /// </summary>
        /// <param name="bot"></param>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public static async Task<Message> ReadMessageAsync(this ITelegramBotClient bot, ChatId chatId)
        {
            var tcs = new TaskCompletionSource<Message>(TaskCreationOptions.RunContinuationsAsynchronously);
            EventHandler<Args.MessageEventArgs> ev = null;
            ev = (s, e) =>
            {
                if (e.Message.Chat.Id == chatId.Identifier || e.Message.Chat.Username == chatId.Username)
                {
                    tcs.TrySetResult(e.Message);
                    bot.OnMessage -= ev;
                }
            };
            bot.OnMessage += ev;
            return await tcs.Task;
        }
    }
}
