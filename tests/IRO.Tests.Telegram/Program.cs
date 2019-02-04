using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using IRO.Tests.Telegram.Controllers;
using IRO.Tests.Telegram.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Extensions.Serialization;
using Telegram.Bot.AspNetPipeline.Mvc.Builder;
using Telegram.Bot.Types.Enums;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace IRO.Tests.Telegram
{
    [Serializable]
    public class NewFoo
    {
        public NewFoo(string str)
        {
            Str = str;
        }

        public string Str { get; }

        //public override string ToString()
        //{
        //    return $"{base.ToString()}({Str})";
        //}
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (false)
            {

                ILogger logger = null;
                logger=new NLogLoggerFactory(new NLogProviderOptions()
                {
                    IncludeScopes = true,
                    CaptureMessageProperties = false,
                    IgnoreEmptyEventId = false
                }).CreateLogger("MyNLog");
                //logger = new LoggerFactory().CreateLogger("MyEmptyLog");

                NestedDiagnosticsContext.Push(new NewFoo("jjjjj"));
                NestedDiagnosticsLogicalContext.Push(new NewFoo("EEEEEEE"));
                while (true)
                {
                    //logger.LogError("{0}", new LazySerializer<NewFoo>(new NewFoo("QQQQQ")));

                    using (logger.BeginScope(new LazySerializer<NewFoo>(new NewFoo("PPPP"))))     
                    {
    
                        logger.LogError("eee");

                        using (logger.BeginScope(new NewFoo("DDDDDD")))
                        {
                            logger.LogError("3333");
                        }
                    }

                    Console.ReadLine();
                }
            }

            var token = BotStaticTestsHelpers.GetToken();
            var bot = new TelegramBotClient(token);
            var botHandler = new BotHandler(bot);

            var botTest = new BotTests_ReadMiddleware();
            botTest.Run(botHandler, true);

            while (true)
            {
                Console.ReadLine();
            }
        }

    }
}
