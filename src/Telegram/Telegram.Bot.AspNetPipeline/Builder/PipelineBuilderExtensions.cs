using System;

namespace Telegram.Bot.AspNetPipeline.Core.Builder
{
    public static class PipelineBuilderExtensions 
    {
        public static void UseMiddlware(this IPipelineBuilder @this, IMiddleware middleware)
        {
            if (middleware == null)
            {
                throw new ArgumentNullException(nameof(middleware));
            }
            @this.Use(middleware.Invoke);
        }
    }
}
