using Telegram.Bot.AspNetPipeline.Mvc.Core.Routing;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.AspNetPipeline.Mvc.Core.Builder
{
    public static class BotMvcBuilderExtensions
    {
        public static void MapRouteAction(
            this IMvcBuilder @this,
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
