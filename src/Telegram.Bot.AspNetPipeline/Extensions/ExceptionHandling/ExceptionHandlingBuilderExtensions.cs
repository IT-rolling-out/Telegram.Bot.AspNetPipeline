using System;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions.ExceptionHandling;

//ReSharper disable CheckNamespace
namespace Telegram.Bot.AspNetPipeline.Extensions
{
    public static class ExceptionHandlingBuilderExtensions
    {
        /// <summary>
        /// In old versions must invoke before all other middleware.
        /// <para></para>
        /// Return false from delegate if you want to throw exception (if not processed).
        /// </summary>
        public static void UseExceptionHandler(
            this IPipelineBuilder @this,
            UpdateProcessingExceptionDelegate updateProcessingExceptionDelegate
            )
        {
            if (updateProcessingExceptionDelegate == null)
                throw new ArgumentNullException(nameof(updateProcessingExceptionDelegate));
            var md = @this.ServiceProvider.GetRequiredService<ExceptionHandlingMiddleware>();
            md.ExceptionHandlers.Insert(0, updateProcessingExceptionDelegate);
        }

        /// <summary>
        /// Mandatory used in <see cref="BotManager"/>.
        /// </summary>
        internal static void UseExceptionHandling(this IPipelineBuilder @this)
        {
            var md = @this.ServiceProvider.GetRequiredService<ExceptionHandlingMiddleware>();
            @this.Use(md.Invoke);
        }

        /// <summary>
        /// Mandatory used in <see cref="BotManager"/>.
        /// </summary>
        internal static void AddExceptionHandling(this ServiceCollectionWrapper @this)
        {
            @this.Services.AddSingleton<ExceptionHandlingMiddleware>();
        }
    }
}
