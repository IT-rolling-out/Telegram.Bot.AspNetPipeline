using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot;
using Telegram.Bot.AspNetPipeline.Extensions.ReadWithoutContext;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Metadata;

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
            await UpdateContext.SendTextMessageAsync("Hi.");

            if (IsModelStateValid)
            {

                await UpdateContext.SendTextMessageAsync(
                    $"Hi. You passed '{str}' with command. Note, default model binder split all command parameters by space," +
                    $"like in CLI, so you can't pass string with command using default model binder."
                );
            }

            await UpdateContext.SendTextMessageAsync("Send message with reply to bot.");
            //Improved bot extension. Allow to await new messages from current chat, just like in console applications.
            var message = await BotExt.ReadMessageAsync(ReadCallbackFromType.CurrentUserReply);

            await UpdateContext.SendTextMessageAsync($"You send: {message.Text}");

            //Use UpdateContext to get more info about current "request".
        }

        [BotRoute("/test_read_without_context")]
        public async Task TestReadWithoutContext()
        {
            await UpdateContext.SendTextMessageAsync("Please send message.");
            var msg = await Bot.ReadMessageAsync(UpdateContext.Chat, NoContextReadCallbackFromType.AnyUser);
            await UpdateContext.SendTextMessageAsync($"You sent '{msg.Text}'.");

            await UpdateContext.SendTextMessageAsync("Please send message with reply.");
            msg = await Bot.ReadMessageAsync(UpdateContext.Chat, NoContextReadCallbackFromType.AnyUserReply);
            await UpdateContext.SendTextMessageAsync($"You sent '{msg.Text}'.");
        }
    }
}
