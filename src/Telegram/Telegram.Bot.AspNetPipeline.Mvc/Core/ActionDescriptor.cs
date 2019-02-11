using Telegram.Bot.AspNetPipeline.Mvc.Routing;

namespace Telegram.Bot.AspNetPipeline.Mvc.Core
{
    /// <summary>
    /// Static data about registered delegate.
    /// </summary>
    public class ActionDescriptor
    {
        public ActionDescriptor(RouteInfo routeInfo)
        {
            RouteInfo = routeInfo;
        }

        public RouteInfo RouteInfo { get; }

    }
}
