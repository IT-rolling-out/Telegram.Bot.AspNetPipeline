using Telegram.Bot.AspNetPipeline.Mvc.Routing;

namespace Telegram.Bot.AspNetPipeline.Mvc.Core
{
    /// <summary>
    /// Routes top object with static data.
    /// Static data about registered delegate.
    /// </summary>
    public class ActionDescriptor
    {
        public ActionDescriptor(RouteActionDelegate handler, RouteInfo routeInfo)
        {
            RouteInfo = routeInfo;
            Handler = handler;
        }

        public RouteInfo RouteInfo { get; }

        public RouteActionDelegate Handler { get; }

        /// <summary>
        /// If all properties is null.
        /// </summary>
        public bool IsEmpty => Handler == null && RouteInfo == null;

        public static ActionDescriptor Empty => new ActionDescriptor(null, null);
    }
}
