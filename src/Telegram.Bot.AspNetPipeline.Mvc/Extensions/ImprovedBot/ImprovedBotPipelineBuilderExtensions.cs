using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot;
using Telegram.Bot.AspNetPipeline.Mvc.Extensions.Main;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing;

//ReSharper disable CheckNamespace
namespace Telegram.Bot.AspNetPipeline.Mvc.Extensions
{
    internal static class ImprovedBotPipelineBuilderExtensions
    {
        /// <summary>
        /// Invoked automatically in mvc middleware.
        /// </summary>
        public static void AddBotExtMvcGlobalValidator(this IPipelineBuilder @this, int botExtOrder, Action<UpdateContext> setGlobalSearchBag)
        {
            IOuterMiddlewaresInformer outerMiddlewaresInformer = null;
            @this.AddBotExtGlobalValidator(async (newCtx, origCtx) =>
            {
                setGlobalSearchBag(newCtx);
                if (outerMiddlewaresInformer == null)
                {
                    var provider = origCtx.Services.GetRequiredService<IOuterMiddlewaresInformerProvider>();
                    outerMiddlewaresInformer = provider.OuterMiddlewaresInformer;
                }
                var mvcWillHandle = await outerMiddlewaresInformer.CheckMvcHasPriorityHandler(newCtx, botExtOrder);
                return mvcWillHandle ? UpdateValidatorResult.AbortWaiter : UpdateValidatorResult.Valid;

            });
        }
    }
}
