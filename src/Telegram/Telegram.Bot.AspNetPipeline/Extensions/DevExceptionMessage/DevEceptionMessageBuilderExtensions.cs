using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.AspNetPipeline.Extensions.DevExceptionMessage
{
    public static class DevEceptionMessageBuilderExtensions
    {
        /// <summary>
        /// Invoke before another middleware.
        /// </summary>
        public static void UseDevEceptionMessage(this IPipelineBuilder @this)
        {
            @this.Use(async (ctx, next) =>
            {
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    if (ex is TaskCanceledException)
                        return;
                    await ctx.SendTextMessageAsync(
                        "```" + ex.ToString() + "```",
                        parseMode: ParseMode.Markdown
                        );
                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
            });
        }
    }
}
