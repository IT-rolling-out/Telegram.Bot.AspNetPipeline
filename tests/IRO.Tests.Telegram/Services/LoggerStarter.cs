using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions.Logging;

namespace IRO.Tests.Telegram.Services
{
    public static class LoggerStarter
    {
        public static void InitLogger(ServiceCollectionWrapper servicesWrap)
        {
            servicesWrap.LoggingAdvancedConfigure(new LoggingAdvancedOptions
            {
                //LoggingWithSerialization = true
            });
            //Default way with Services.AddLogging doesn't work for me.
            servicesWrap.Services.AddSingleton<ILoggerFactory>(new LoggerFactory(new ILoggerProvider[]
            {
                new NLogLoggerProvider(
                    new NLogProviderOptions()
                    {
                        IncludeScopes = true,
                        CaptureMessageProperties = true
                    })
            }));

        }
    }
}
