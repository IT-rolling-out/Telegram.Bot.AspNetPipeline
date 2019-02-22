using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions.ExceptionHandling;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.AspNetPipeline.Extensions.DevExceptionMessage
{
    public static class DevEceptionMessageBuilderExtensions
    {
        /// <summary>
        /// In old versions must invoke before another middleware, but after UseExceptionHandler.
        /// <para></para>
        /// Will send message with exception to chat.
        /// </summary>
        public static void UseDevEceptionMessage(this IPipelineBuilder @this)
        {
            @this.UseExceptionHandler(async (ctx, ex) =>
            {
                if (!(ex is TaskCanceledException))
                {
                    var msg=ex.ToString();
                    //Max for telegram is 4096 UTF8  characters.
                    if (msg.Length > 4080)
                        msg = msg.Remove(4080) + "...";
                    await UpdateContextFrequentlyUsedExtensions.SendTextMessageAsync(ctx, "```" + msg + "```",
                        parseMode: ParseMode.Markdown
                    );
                }
                return false;
            });
        }
    }
}
