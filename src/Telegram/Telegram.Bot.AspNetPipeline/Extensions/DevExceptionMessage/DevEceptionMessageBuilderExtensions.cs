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
                    var exceptionText = ex.ToString();

                    //Send messages.
                    var utfText = exceptionText.ToUTF8();
                    var maxLength = FrequentlyUsedExtensions.MaxTelegramMessageLength - 6;
                    while (true)
                    {
                        if (utfText.Length > maxLength)
                        {
                            string currentMessage = utfText.Remove(maxLength);
                            utfText = utfText.Substring(maxLength);
                            await ctx.SendTextMessageAsync(
                                "```" + currentMessage + "```",
                                parseMode: ParseMode.Markdown
                                );
                        }
                        else
                        {
                            await ctx.SendTextMessageAsync(
                                "```" + utfText + "```",
                                parseMode: ParseMode.Markdown
                                );
                            await ctx.SendTextMessageAsync(
                               "====================",
                               parseMode: ParseMode.Markdown
                               );
                        }
                    }
                }
                return false;
            });
        }
    }
}
