using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot;
using Telegram.Bot.AspNetPipeline.Mvc.Extensions.Main;

//ReSharper disable CheckNamespace
namespace Telegram.Bot.AspNetPipeline.Mvc.Extensions
{
    internal static class PipelineBuilderExtensions
    {
        /// <summary>
        /// Invoked automatically in mvc middleware.
        /// </summary>
        public static void AddBotExtMvcGlobalValidator(this IPipelineBuilder @this, int botExtOrder)
        {
            IOuterMiddlewaresInformer outerMiddlewaresInformer=null;
            @this.AddBotExtGlobalValidator(async (newCtx, origCtx) =>
            {
                if (outerMiddlewaresInformer == null)
                {
                    var provider=origCtx.Services.GetRequiredService<IOuterMiddlewaresInformerProvider>();
                    outerMiddlewaresInformer=provider.OuterMiddlewaresInformer;
                }
                var mvcWillHandle=await outerMiddlewaresInformer.CheckMvcHasPriorityHandler(newCtx, botExtOrder);
                return mvcWillHandle ? UpdateValidatorResult.AbortWaiter : UpdateValidatorResult.Valid; 

            });
        }
    }
}
