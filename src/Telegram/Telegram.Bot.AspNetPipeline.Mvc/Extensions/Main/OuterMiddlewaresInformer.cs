using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Routers;

namespace Telegram.Bot.AspNetPipeline.Mvc.Extensions.Main
{
    public class OuterMiddlewaresInformer:IOuterMiddlewaresInformer
    {
        readonly MainRouter _mainRouter;

        public OuterMiddlewaresInformer(MainRouter mainRouter)
        {
            _mainRouter = mainRouter;
        }
       
        /// <summary>
        /// If mvc has method with bigger priority - return true.
        /// <para></para>
        /// Lower number mean bigger priority.
        /// </summary>
        public async Task<bool> CheckMvcHasPriorityHandler(UpdateContext updateContext, int yourMethodOrder)
        {
            var routingContext = new RoutingContext(updateContext);
            await _mainRouter.RouteAsync(routingContext);
            if (routingContext.ActionDescriptor != null)
            {
                return routingContext.ActionDescriptor.RouteInfo.Order <= yourMethodOrder;
            }
            return false;
        }
    }
}

