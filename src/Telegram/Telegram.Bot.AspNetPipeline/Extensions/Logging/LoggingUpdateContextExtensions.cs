using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot;
using Telegram.Bot.AspNetPipeline.Extensions.Serialization;

namespace Telegram.Bot.AspNetPipeline.Extensions.Logging
{
    public static class LoggingUpdateContextExtensions
    {
        const string LazySerializerPropertyName = "_LazySerializer";

        /// <summary>
        /// Return LazySerializer or UpdateContext.
        /// Used to enable custom serialization for UpdateContext, when it used as logger scope.
        /// More info in <see cref="LoggingAdvancedOptions"/>.
        /// </summary>
        public static object GetLoggerScope(this UpdateContext @this)
        {
            var hiddenCtx = HiddenUpdateContext.Resolve(@this);
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
    }
}
