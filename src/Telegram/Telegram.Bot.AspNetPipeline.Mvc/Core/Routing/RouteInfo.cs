using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.AspNetPipeline.Mvc.Core.Routing
{
    /// <summary>
    /// Used to describe route.
    /// <para></para>
    /// Something like AttributeRouteInfo from asp.net.
    /// Can be used with controller method or any other route (ActionDescriptor) in mvc.
    /// </summary>
    public class RouteInfo
    {
        public RouteInfo(
            string template=null, 
            int order=0, 
            string name=null, 
            UpdateType[] updateTypes=null
            )
        {
            Template = template;
            Order = order;
            Name = name;
            UpdateTypes = updateTypes;
        }

        /// <summary>
        /// The route template. May be null if the action has no attribute routes.
        /// </summary>
        public string Template { get; }

        /// <summary>
        /// Gets the order of the route associated with a given action. This property determines
        /// the order in which routes get executed. Routes with a lower order value are tried first. In case a route
        /// doesn't specify a value, it gets a default order of 0.
        /// </summary>
        public int Order { get; }

        public string Name { get;  }

        public UpdateType[] UpdateTypes { get; }
    }
}
