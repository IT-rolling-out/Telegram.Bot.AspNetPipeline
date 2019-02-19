using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Mvc;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Metadata;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace IRO.Tests.Telegram.Controllers
{
    public class BotFatherSampleController : BotController
    {
        [BotRoute("/newbot", UpdateType.Message)]
        public async Task NewBot()
        {
            await Bot.SendTextMessageAsync(Chat.Id, "Enter bot name: ");
            Message msg = await BotExt.ReadMessageAsync();
            var name = msg.Text;
            await Bot.SendTextMessageAsync(Chat.Id, "Enter bot nikname: ");
            msg = await BotExt.ReadMessageAsync();
            var nick = msg.Text;

            //Creating bot started...
            //If operation is too long, you can use CancellationToken to calcel it when cancellation requested, just like ReadMessageAsync do.
            UpdateProcessingAborted.ThrowIfCancellationRequested();
            //Creating bot finished...

            await Bot.SendTextMessageAsync(Chat.Id, "Bot created.");

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
            await Bot.SendTextMessageAsync(Chat.Id, "Commands list:\n" +
                                                    "/newbot - NewBot,\n" +
                                                    "/help - Help");
        }

        /// <summary>
        /// Will process all requests, but only if other command not executing now,
        /// because have priority lower than default (0).
        /// So will not cancel NewBot.
        /// NOTE: Bigger Order mean lower priority, asp.net naming.
        /// </summary>
        [BotRoute(Order = 2, Name = "Default")]
        public async Task Default()
        {
            await Bot.SendTextMessageAsync(Chat.Id, "Hi, i am BotFather.");
        }
    }
}
