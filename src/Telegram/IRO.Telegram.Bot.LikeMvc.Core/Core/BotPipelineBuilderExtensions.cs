using System;

namespace IRO.Telegram.Bot.ProcessingPipeline.Core
{
    public static class BotPipelineBuilderExtensions 
    {
        public static void UseMiddlware(this IBotPipelineBuilder @this, IMiddleware middleware)
        {
            if (middleware == null)
            {
                throw new ArgumentNullException(nameof(middleware));
            }
            @this.Use(middleware.Invoke);
        }
    }
}
