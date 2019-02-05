using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Extensions.Logging;

namespace Telegram.Bot.AspNetPipeline.Extensions.ExceptionHandler
{
    public static class ExceptionHandlingBuilderExtensions
    {
        /// <summary>
        /// Invoke before all another middleware.
        /// <para>Return false from delegate if want to throw exception.</para>
        /// </summary>
        public static void UseExceptionHandler(
            this IPipelineBuilder @this, 
            UpdateProcessingExceptionDelegate updateProcessingExceptionDelegate
            )
        {
            if (updateProcessingExceptionDelegate == null)
                throw new ArgumentNullException(nameof(updateProcessingExceptionDelegate));
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
                    ctx.Logger().LogError("Exception catched in exception handler '{0}'.", ex);
                    var edi=ExceptionDispatchInfo.Capture(ex);
                    var handled=await updateProcessingExceptionDelegate(ctx, ex);
                    if (!handled)
                    {
                        edi.Throw();
                    }
                }
            });
        }
    }
}
