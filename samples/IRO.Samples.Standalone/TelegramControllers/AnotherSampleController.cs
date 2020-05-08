using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot;
using Telegram.Bot.AspNetPipeline.Extensions.ReadWithoutContext;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Metadata;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace IRO.Samples.Standalone.TelegramControllers
{
    public class AnotherSampleController : BotController
    {
        /// <summary>
        /// Controller methods must always return Task. 
        /// </summary>
        [BotRoute("/hi")]
        public async Task Hi(string str)
        {
            //Use dependency injection just like you do in ASP.NET.
            //Use extensions to make your life eathier.
            //Some of extension services are included as controller properties:
            // * logger
            // * BotExt (improved bot)
            // * mvc features
            // ...

            //Access current telegram Update as property of controller.

            //Extension too. Send message to current chat.
            await SendTextMessageAsync("Hi.");

            if (IsModelStateValid)
            {

                await SendTextMessageAsync(
                    $"Hi. You passed '{str}' with command. Note, default model binder split all command parameters by space," +
                    $"like in CLI, so you can't pass string with command using default model binder."
                );
            }

            await UpdateContext.SendTextMessageAsync("Send message with reply to bot.");
            //Improved bot extension. Allow to await new messages from current chat, just like in console applications.
            var message = await BotExt.ReadMessageAsync(ReadCallbackFromType.CurrentUserReply);

            await SendTextMessageAsync($"You send: {message.Text}");

            //Use UpdateContext to get more info about current "request".
        }

        [BotRoute("/test_read_no_ctx")]
        public async Task TestReadWithoutContext()
        {
            await SendTextMessageAsync("Please send message.");
            var msg = await Bot.ReadMessageAsync(ChatId, NoContextReadCallbackFromType.AnyUser);
            await SendTextMessageAsync($"You sent '{msg.Text}'.");

            await SendTextMessageAsync("Please send message with reply.");
            msg = await Bot.ReadMessageAsync(ChatId, NoContextReadCallbackFromType.AnyUserReply);
            await SendTextMessageAsync($"You sent '{msg.Text}'.");
        }

        [BotRoute("/test_query_read")]
        public async Task TestCallbackQuery()
        {
            var replyMarkup = new InlineKeyboardMarkup(
                new InlineKeyboardButton[][]
                {
                    new InlineKeyboardButton[] { "one", "two" },
                    new InlineKeyboardButton[] { "three", "four" },
                }
            );
            await Bot.SendTextMessageAsync(
                ChatId,
                "Read inline query test.",
                replyMarkup: replyMarkup
            );

            var readUpd = await BotExt.ReadUpdateAsync(async (newCtx, currentCtx) =>
            {
                if (newCtx.Update.Type == UpdateType.CallbackQuery)
                    return UpdateValidatorResult.Valid;
                return UpdateValidatorResult.ContinueWaiting;
            });
            await SendTextMessageAsync($"You chosen '{readUpd.CallbackQuery.Data}'.");

        }

        [BotRoute("/test_query_read_no_ctx")]
        public async Task TestCallbackQueryWithoutContext()
        {
            var replyMarkup = new InlineKeyboardMarkup(
                new InlineKeyboardButton[][]
                {
                    new InlineKeyboardButton[] { "one", "two" },
                    new InlineKeyboardButton[] { "three", "four" },
                }
            );
            await Bot.SendTextMessageAsync(
                ChatId,
                "Read inline query test, no UpdateContext.",
                replyMarkup: replyMarkup
            );

            var readUpd = await Bot.ReadUpdateAsync(Chat.Id, (chatId, upd) =>
            {
                if (upd.Type == UpdateType.CallbackQuery)
                    return true;
                return false;
            });
            await SendTextMessageAsync($"You chosen '{readUpd.CallbackQuery.Data}'.");
        }

        [BotRoute("/test_restore_markup")]
        public async Task TestRestoreMarkup()
        {
            await SendTextMessageAsync($"Send me something like:\n\n" +
                                       $"`Code str` \n```Code``` \n*Bold* \n_Italic_ \n[google link](https://google.com)");

            var readMsg = await BotExt.ReadMessageAsync();
            var markdownText = readMsg.RestoreMarkdown();
            await SendTextMessageAsync(markdownText, parseMode: ParseMode.MarkdownV2);
        }

        /// <summary>
        /// Will process all requests, but only if other command not executing now,
        /// because have priority lower than default (0).
        /// So will not cancel NewBot.
        /// NOTE: Bigger Order mean lower priority, ASP.NET naming.
        /// </summary>
        [BotRoute(Order = 1, Name = "Default")]
        public async Task Default()
        {
            await Bot.SendTextMessageAsync(ChatId, "Hi, i am default handler.");
        }
    }
}
