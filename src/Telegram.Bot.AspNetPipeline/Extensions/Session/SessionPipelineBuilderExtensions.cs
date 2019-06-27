using System;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Extensions.Session;

//ReSharper disable CheckNamespace
namespace Telegram.Bot.AspNetPipeline.Extensions
{
    public static class SessionPipelineBuilderExtensions
    {
        public static void AddSessionStorage(this ServiceCollectionWrapper @this, Func<IServiceProvider, ISessionStorageProvider> func)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));
            @this.Services.AddSingleton<ISessionStorageProvider>(
                func
                );
        }

        public static void AddSessionStorage<TSessionStorageProvider>(this ServiceCollectionWrapper @this)
            where TSessionStorageProvider : class, ISessionStorageProvider
        {
            @this.Services.AddSingleton<ISessionStorageProvider, TSessionStorageProvider>();
        }

        /// <summary>
        /// Init default <see cref="RamSessionStorageProvider"/>. Used by default.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="sessionTimeout">Default is 30 minutes.</param>
        /// <param name="checkInterval">Default is 5 minutes.</param>
        public static void AddRamSessionStorage(
            this ServiceCollectionWrapper @this,
            TimeSpan? sessionTimeout = null,
            TimeSpan? checkInterval = null
            )
        {
            var sessionStorage = new RamSessionStorageProvider(sessionTimeout, checkInterval);
            @this.Services.AddSingleton<ISessionStorageProvider>(
                sessionStorage
                );
            @this.Services.AddSingleton<RamSessionStorageProvider>(
                sessionStorage
                );
            @this.RegisterForDispose<RamSessionStorageProvider>();
        }
    }
}
