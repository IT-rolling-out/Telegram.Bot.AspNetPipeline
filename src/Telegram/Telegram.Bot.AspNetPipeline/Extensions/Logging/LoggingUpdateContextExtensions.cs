using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot;
using Telegram.Bot.AspNetPipeline.Extensions.Serialization;

namespace Telegram.Bot.AspNetPipeline.Extensions.Logging
{
    public static class LoggingUpdateContextExtensions
    {
        const string LazySerializerPropertyName = "_LazySerializer";

        const string LoggerPropertyName = "_Logger";

        /// <summary>
        /// Return LazySerializer or UpdateContext.
        /// Used to enable custom serialization for UpdateContext, when it used as logger scope.
        /// More info in <see cref="LoggingAdvancedOptions"/>.
        /// </summary>
        public static object GetLoggerScope(this UpdateContext @this)
        {
            var hiddenCtx = @this.HiddenContext();
            if (hiddenCtx.LoggingAdvancedOptions.LoggingWithSerialization)
            {
                if (@this.Properties.TryGetValue(LazySerializerPropertyName, out var lazySerializerNotCasted))
                {
                    return (LazySerializer<UpdateContext>)lazySerializerNotCasted;
                }
                else
                {
                    var lazySerializer = hiddenCtx.LoggingAdvancedOptions.LazySerializerFactory.Create(@this);
                    @this.Properties[LazySerializerPropertyName] = lazySerializer;
                    return lazySerializer;
                }
            }
            else
            {
                return @this;
            }
        }

        /// <summary>
        /// Fast way to log message in UpdateContext scope.
        /// <para></para>
        /// Better to use ILoggerFactory injection, but you can use current service for fast loggin.
        /// </summary>
        public static ILogger Logger(this UpdateContext @this)
        {
            if (@this.Properties.TryGetValue(LoggerPropertyName, out var loggerNotCasted))
            {
                return (ILogger)loggerNotCasted;
            }
            else
            {
                var loggerFactory = @this.Services.GetService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger(GetLoggerScope(@this).ToString());
                @this.Properties[LoggerPropertyName] = logger;
                return logger;
            }
        }
    }
}
