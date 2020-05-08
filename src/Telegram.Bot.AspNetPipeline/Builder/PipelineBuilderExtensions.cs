using System;
using Microsoft.Extensions.DependencyInjection;

namespace Telegram.Bot.AspNetPipeline.Builder
{
    public static class PipelineBuilderExtensions 
    {
        /// <summary>
        /// Resolves middleware form ServiceProvider and use it.
        /// </summary>
        public static void UseMiddleware<TMiddleware>(this IPipelineBuilder @this)
            where TMiddleware : IMiddleware
        {
            var md = @this.ServiceProvider.GetRequiredService<TMiddleware>();
            @this.UseMiddleware(md);
        }

        public static void UseMiddleware(this IPipelineBuilder @this, IMiddleware middleware)
        {
            if (middleware == null)
                throw new ArgumentNullException(nameof(middleware));
            @this.Use(middleware.Invoke);
        }

    }
}
