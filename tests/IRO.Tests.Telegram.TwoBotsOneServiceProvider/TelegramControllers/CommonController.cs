using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IRO.Tests.Telegram.TwoBotsOneServiceProvider.Services;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Metadata;

namespace IRO.Tests.Telegram.TwoBotsOneServiceProvider.TelegramControllers
{
    public class CommonController:BotController
    {
        readonly CommonService _commonService;

        public CommonController(CommonService commonService)
        {
            _commonService = commonService;
        }

        [BotRoute("/test_common_service")]
        public async Task TestCommonService()
        {
            await SendTextMessageAsync(_commonService.GetText());
        }

        [BotRoute("/session_test")]
        public async Task TestCommonService(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                var sessionVar = await Session.GetOrDefault<string>("sessionVar");
                await SendTextMessageAsync($"Session variable: {sessionVar}");
            }
            else
            {
                await Session.Set("sessionVar", str);
                await SendTextMessageAsync($"Set session variable to: {str}");
            }
        }

        /// <summary>
        /// Controller methods must always return Task. 
        /// </summary>
        [BotRoute("/read_msg_test")]
        public async Task ReadMsgTest()
        {
            await UpdateContext.SendTextMessageAsync("Send me a message.");
            var message = await BotExt.ReadMessageAsync(ReadCallbackFromType.CurrentUser);
            await SendTextMessageAsync($"You send: {message.Text}");
        }


    }
}
