using System;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Builder;

namespace Telegram.Bot.AspNetPipeline.Extensions.ExceptionHandling
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
