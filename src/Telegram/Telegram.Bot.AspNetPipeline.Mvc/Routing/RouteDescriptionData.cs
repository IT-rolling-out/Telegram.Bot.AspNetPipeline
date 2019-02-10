using Telegram.Bot.AspNetPipeline.Mvc.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing
{
    /// <summary>
    /// Main route object, contains all data about one route.
    /// </summary>
    public struct RouteDescriptionData
    {
        public RouteDescriptionData(RouteActionDelegate handler, RouteInfo routeInfo)
        {
            Handler = handler;
            RouteInfo = routeInfo;
        }

        public RouteActionDelegate Handler { get; set; }

        public RouteInfo RouteInfo { get; set; }

        /// <summary>
        /// If all properties is null.
        /// </summary>
        public bool IsEmpty => Handler==null && RouteInfo==null;

        public static RouteDescriptionData Empty => new RouteDescriptionData();

        public override bool Equals(object obj)
        {
            if (obj is RouteDescriptionData rdd)
            {
                return Handler == rdd.Handler && RouteInfo==rdd.RouteInfo;
            }
            return base.Equals(obj);
        }
    }
}
