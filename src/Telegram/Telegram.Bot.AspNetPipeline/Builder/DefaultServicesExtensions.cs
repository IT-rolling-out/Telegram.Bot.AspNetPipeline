using System;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Services;

namespace Telegram.Bot.AspNetPipeline.Builder
{
    /// <summary>
    /// Use it to make work with services more clear.
    /// </summary>
    public static class DefaultServicesExtensions
    {
        /// <summary>
        /// Used in <see cref="BotManager"/>.
        /// Override default singleton.
        /// </summary>
        public static void AddPendingExceededChecker<TPendingExceededChecker>(this ServiceCollectionWrapper @this)
            where TPendingExceededChecker : class, IPendingExceededChecker
        {
            @this.Services.AddSingleton<IPendingExceededChecker, TPendingExceededChecker>();
        }

        /// <summary>
        /// Used in <see cref="BotManager"/>.
        /// Override default singleton.
        /// </summary>
        public static void AddPendingExceededChecker(this ServiceCollectionWrapper @this, Func<IServiceProvider, IPendingExceededChecker> func)
        {
            @this.Services.AddSingleton<IPendingExceededChecker>(func);
        }

        /// <summary>
        /// Used in <see cref="BotManager"/>.
        /// Override default singleton.
        /// </summary>
        public static void AddExecutionManager<TExecutionManager>(this ServiceCollectionWrapper @this)
            where TExecutionManager : class, IExecutionManager
        {
            @this.Services.AddSingleton<IExecutionManager, TExecutionManager>();
        }

        /// <summary>
        /// Used in <see cref="BotManager"/>.
        /// Override default singleton.
        /// </summary>
        public static void AddPendingExceededChecker(this ServiceCollectionWrapper @this, Func<IServiceProvider, IExecutionManager> func)
        {
            @this.Services.AddSingleton<IExecutionManager>(func);
        }
    }
}
