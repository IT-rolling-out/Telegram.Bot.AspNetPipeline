using System;
using Microsoft.Extensions.DependencyInjection;

namespace Telegram.Bot.AspNetPipeline.Builder
{
    public static class PipelineBuilderExtensions 
    {
        /// <summary>
        /// Resolves middleware form ServiceProvider and use it.
        /// </summary>
        public static void UseMiddlware<TMiddleware>(this IPipelineBuilder @this)
            where TMiddleware:IMiddleware
        {
            var md=@this.ServiceProvider.GetService<TMiddleware>();
            @this.Use(md.Invoke);
        }
    }
}
