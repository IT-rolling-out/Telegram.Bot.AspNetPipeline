using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Extensions.Session;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Metadata;

namespace IRO.Tests.Telegram.Controllers
{
    public class SessionTestController : BotController
    {
        [BotRoute("/set")]
        public async Task Set(string key, string val)
        {
            await Session.Set(key, val);
            var restoredVal = await Session.GetOrDefault<string>(key);
            await SendTextMessageAsync($"'{key}' saved as '{restoredVal}'.");
        }

        [BotRoute("/show_all")]
        public async Task ShowAll()
        {
            string msg = "Session \n\n";
            foreach (var key in Session.Keys)
            {
                var restoredVal = await Session.GetOrDefault<string>(key);
                msg += $"{key} : {restoredVal}\n\n";
            }

            await Bot.SendTextMessageInPartsAsync(Chat.Id, msg);
        }

        [BotRoute("/clear")]
        public async Task Clear()
        {
            Session.Clear();
            await SendTextMessageAsync($"Removed.");
        }
    }
}
