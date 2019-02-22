using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Telegram.Bot;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace IRO.UnitTests.Telegram
{
    public static class BotUnitTestsHelpers
    {
        public static IPipelineBuilder CreatePipelineBuilder(IServiceProvider serv)
        {
            var fullTypeName = typeof(IPipelineBuilder).Namespace + ".PipelineBuilder";
            var t = typeof(IPipelineBuilder).Assembly.GetType(fullTypeName);
            var res = (IPipelineBuilder)Activator.CreateInstance(t, serv);
            return res;
        }


        public static string GetToken()
        {
            try
            {
                //Read token from gitignored file.
                var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\..\\",
                    "test_token.txt"));
                var token = File.ReadAllText(path).Trim();
                if (string.IsNullOrWhiteSpace(token))
                    throw new Exception();
                return token;
            }
            catch(Exception ex)
            {
                throw new Exception("Wrong token. Please, check 'test_token.txt' exists in solution folder.", ex);
            }
        }

        public static BotManager BotManager()
        {
            var token = GetToken();
            var bot = new TelegramBotClient(token);
            var botManager = new BotManager(bot);
            return botManager;
        }

        public static UpdateContext CreateUpdateContextMock(IServiceProvider services)
        {
            var bot = new Mock<ITelegramBotClient>();
            var update = new Mock<Update>();
            var user = new Mock<User>();
            var cts = new CancellationTokenSource();
            var ctx = new UpdateContext(
                update.Object,
                new BotClientContext(bot.Object, user.Object),
                services,
                cts.Token
            );
            return ctx;
        }
    }
}
