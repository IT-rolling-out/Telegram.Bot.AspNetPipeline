using System;
using System.Runtime.ExceptionServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Extensions.Logging;

namespace Telegram.Bot.AspNetPipeline.Extensions.ExceptionHandler
{
    public static class ExceptionHandlingBuilderExtensions
    {
        /// <summary>
        /// Invoke before all another middleware.
        /// <para></para>
        /// Return false from delegate if want to throw exception.
        /// </summary>
        public static void UseExceptionHandler(
            this IPipelineBuilder @this,
            UpdateProcessingExceptionDelegate updateProcessingExceptionDelegate
            )
        {
            if (updateProcessingExceptionDelegate == null)
                throw new ArgumentNullException(nameof(updateProcessingExceptionDelegate));
            var md = @this.ServiceProvider.GetService<ExceptionHandlingMiddleware>();
            md.ExceptionHandlers.Insert(0, updateProcessingExceptionDelegate);
        }

        /// <summary>
        /// Mandatory used.
        /// </summary>
        internal static void UseExceptionHandling(this IPipelineBuilder @this)
        {
            var md = @this.ServiceProvider.GetService<ExceptionHandlingMiddleware>();
            @this.Use(md.Invoke);
        }

        /// <summary>
        /// Mandatory used.
        /// </summary>
        internal static void AddExceptionHandling(this ServiceCollectionWrapper @this)
        {
            @this.Services.AddSingleton<ExceptionHandlingMiddleware>();
        }
    }
}
