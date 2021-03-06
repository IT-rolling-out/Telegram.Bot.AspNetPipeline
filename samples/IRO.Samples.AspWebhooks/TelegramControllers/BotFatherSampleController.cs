﻿using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Metadata;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace IRO.Samples.AspWebhooks.TelegramControllers
{
    public class BotFatherSampleController : BotController
    {
        [BotRoute("/newbot", UpdateType.Message)]
        public async Task NewBot()
        {
            await Bot.SendTextMessageAsync(ChatId, "Enter bot name: ");
            Message msg = await BotExt.ReadMessageAsync();
            var name = msg.Text;
            await Bot.SendTextMessageAsync(ChatId, "Enter bot nikname: ");
            msg = await BotExt.ReadMessageAsync();
            var nick = msg.Text;

            //Creating bot started...
            //If operation is too long, you can use CancellationToken to calcel it when cancellation requested, just like ReadMessageAsync do.
            UpdateProcessingAborted.ThrowIfCancellationRequested();
            //Creating bot finished...

            await Bot.SendTextMessageAsync(ChatId, "Bot created.");

            //In mvc middleware called by default if found command or read-callback.
            //Next middleware can ignore it.
            UpdateContext.Processed();
        }

        /// <summary>
        /// Will cancel NewBot.
        /// </summary>
        [BotRoute("/help", UpdateType.Message, Name = "Help")]
        public async Task Help()
        {
            await Bot.SendTextMessageAsync(ChatId, "Commands list:\n" +
                                                    "/newbot - NewBot,\n" +
                                                    "/help - Help");
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
            await Bot.SendTextMessageAsync(ChatId, "Hi, i am BotFather.");
        }
    }
}
