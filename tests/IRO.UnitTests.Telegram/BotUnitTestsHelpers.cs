using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json.Linq;
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
            return LoadJson()["token"].ToObject<string>();
        }

        static JToken LoadJson()
        {
            try
            {
                string jsonStr = null;
                try
                {
                    var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\..\\",
                        "test_token.json"));
                    jsonStr = File.ReadAllText(path);
                }
                catch
                {
                    var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..\\..\\..\\..\\..\\..\\",
                        "test_token.json"));
                    jsonStr = File.ReadAllText(path);
                }
                var jToken = JToken.Parse(jsonStr);
                return jToken;
            }
            catch (Exception ex)
            {
                throw new Exception("Wrong token. Please, check 'test_token.json' exists in solution folder.", ex);
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
