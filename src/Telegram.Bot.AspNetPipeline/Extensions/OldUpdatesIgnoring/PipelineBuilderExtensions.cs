using System;
using Telegram.Bot.AspNetPipeline.Builder;

//ReSharper disable CheckNamespace
namespace Telegram.Bot.AspNetPipeline.Extensions
{
    public static class PipelineBuilderExtensions
    {
        /// <summary>
        /// Ignore updates, that older than passed datetime.
        /// Now works only for messages and channel posts.
        /// Default is <see cref="DateTime.UtcNow"/>.
        /// <para></para>
        /// Invoke before middleware you want ignore.
        /// <para></para>
        /// Useful when you want to ignore messages sended when bot was disabled.
        /// </summary>
        public static void UseOldUpdatesIgnoring(this IPipelineBuilder @this, DateTime? dateTime=null)
        {
            var notNullableDateTime = dateTime ?? DateTime.UtcNow;
            @this.Use(async (ctx, next) =>
            {
             
                DateTime? updateTime=ctx.Update.ChannelPost?.Date
                    ?? ctx.Update.Message?.Date;
                if (updateTime != null && updateTime < notNullableDateTime)
                {
                    //Ignoring...
                    ctx.ForceExit();
                }
                else
                {
                    await next();
                }
            });
        }
    }
}
