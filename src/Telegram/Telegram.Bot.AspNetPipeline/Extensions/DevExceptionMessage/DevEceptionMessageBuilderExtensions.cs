using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.Types.Enums;

//ReSharper disable CheckNamespace
namespace Telegram.Bot.AspNetPipeline.Extensions
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
                    //Max for telegram is 4096 UTF8  characters.
                    if (utfText.Length > 4080)
                        utfText = utfText.Remove(4080) + "...";
                    await ctx.SendTextMessageAsync( 
                        "```\n" + utfText + " ```",
                        parseMode: ParseMode.Markdown
                    );
                }
                return false;
            });
        }
    }
}
