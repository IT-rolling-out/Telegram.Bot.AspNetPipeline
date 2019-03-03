using System;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Extensions.Logging;

//ReSharper disable CheckNamespace
namespace Telegram.Bot.AspNetPipeline.Extensions
{ 
    public static class LoggingPipelineBuilderExtensions
    {
        /// <summary>
        /// Here you can edit logging settings specialized for current library.
        /// </summary>
        public static void LoggingAdvancedConfigure(this ServiceCollectionWrapper @this, LoggingAdvancedOptions options)
        {
            Func<LoggingAdvancedOptions> func = () => options;
            @this.Services.AddSingleton(func);
        }
    }

}
