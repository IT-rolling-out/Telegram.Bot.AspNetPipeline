using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IRO.Tests.Telegram.Services;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Metadata;

namespace IRO.Tests.Telegram.Controllers
{
    public class TestController:BotController
    {
        public TestController(ISomeScopedService someScopedService)
        {
            
        }

        [BotRoute("/param")]
        public async Task ParamTest(int num, bool boolean, string str)
        {
            await UpdateContext.SendTextMessageAsync($"num: {num}\nboolean: {boolean}\nstr: '{str}'" +
                                                     $"\n\nall valid: {IsModelStateValid}");
        }

        [BotRoute(Order = 1)]
        public async Task WithoutCmd(int num, bool boolean, string str)
        {
            await UpdateContext.SendTextMessageAsync($"num: {num}\nboolean: {boolean}\nstr: '{str}'" +
                                                      $"\n\nall valid: {IsModelStateValid}");
        }
    }
}
