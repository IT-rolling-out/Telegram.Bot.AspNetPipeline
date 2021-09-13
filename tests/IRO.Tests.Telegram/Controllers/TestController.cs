using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IRO.Tests.Telegram.Services;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Metadata;
using Telegram.Bot.Types.Enums;

namespace IRO.Tests.Telegram.Controllers
{
    public class TestController : BotController
    {
        public TestController(ISomeScopedService someScopedService)
        {
            //DI test.
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
            return;
            await UpdateContext.SendTextMessageAsync($"num: {num}\nboolean: {boolean}\nstr: '{str}'" +
                                                     $"\n\nall valid: {IsModelStateValid}");
        }

        [BotRoute("/ex")]
        public async Task ExceptionsTest()
        {
            throw new Exception("Some exception message.");
        }

        [BotRoute("/parts_send")]
        public async Task PartsSendTest()
        {
            var str = "Этот текст отправлен через SendTextMessageInPartsAsync, проверяем метод.\n";
            for (int i = 0; i < 6000; i++)
            {
                str += "ы";
            }

            await Bot.SendTextMessageInPartsAsync(ChatId, str);
        }
    }
}
