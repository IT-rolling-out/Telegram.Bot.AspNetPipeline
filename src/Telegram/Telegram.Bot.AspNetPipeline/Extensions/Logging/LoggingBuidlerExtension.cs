using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Internal;
using Telegram.Bot.AspNetPipeline.Core;

namespace Telegram.Bot.AspNetPipeline.Extensions.Logging
{
    public static class LoggingBuidlerExtension
    {
        public static void AddLogger<ILoggerFactory>()
        {
            ILoggerProvider loggerProvider;
            var loggerFactory = new LoggerFactory();
            var logger = loggerFactory.CreateLogger("GlobalLogger");
            var stateObj = 100;
            using (logger.BeginScope(stateObj))
            {
                logger.LogError("Eee");
            }
        }
    }

    public static class LoggingBotExtensions
    {
        /// <summary>
        /// Resolve logger when current context is scope.
        /// </summary>
        //public static ILogger Logger(this UpdateContext @this)
        //{
        //    ILoggingBuilder lb;
        //    LoggerFactory loggerFactory = new LoggerFactory();
           
        //}
    }
}