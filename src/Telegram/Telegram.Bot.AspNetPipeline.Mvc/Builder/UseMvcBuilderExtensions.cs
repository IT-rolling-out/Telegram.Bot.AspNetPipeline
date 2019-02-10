using IRO.Common.Collections;
using Telegram.Bot.AspNetPipeline.Mvc.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.AspNetPipeline.Mvc.Builder
{
    public static class UseMvcBuilderExtensions
    {
        public static void MapRouteAction(
            this IUseMvcBuilder @this,
            RouteActionDelegate routeAction,
            string template = null,
            int order = 0,
            string name = null,
            UpdateType[] updateTypes = null
            )
        {
            var routeInfo = new RouteInfo(template, order, name, updateTypes);
            @this.MapRouteAction(routeAction, routeInfo);
        }
    }
}
