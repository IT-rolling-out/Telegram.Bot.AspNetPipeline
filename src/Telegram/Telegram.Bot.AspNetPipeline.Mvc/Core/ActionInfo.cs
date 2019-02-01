using Telegram.Bot.AspNetPipeline.Mvc.Routing;

namespace Telegram.Bot.AspNetPipeline.Mvc.Core
{
    public class ActionInfo
    {
        public ActionInfo(RouteInfo routeInfo)
        {
            RouteInfo = routeInfo;
        }

        public RouteInfo RouteInfo { get; }
    }
}
