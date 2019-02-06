using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions.ExceptionHandler;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.AspNetPipeline.Extensions.DevExceptionMessage
{
    public static class DevEceptionMessageBuilderExtensions
    {
        /// <summary>
        /// Invoke before another middleware, but after UseExceptionHandler.
        /// </summary>
        public static void UseDevEceptionMessage(this IPipelineBuilder @this)
        {
            @this.UseExceptionHandler(async (ctx, ex) =>
            {
                if (!(ex is TaskCanceledException))
                {
                    await ctx.SendTextMessageAsync(
                        "```" + ex.ToString() + "```",
                        parseMode: ParseMode.Markdown
                    );
                }
                return false;
            });
        }
    }
}
