using System;
using System.Collections.Generic;
using Telegram.Bot.AspNetPipeline.Mvc.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Routers;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing;

namespace Telegram.Bot.AspNetPipeline.Mvc.Builder
{
    public interface IUseMvcBuilder
    {
        /// <summary>
        /// Default routers.  
        /// </summary>
        IList<IRouter> Routers { get; }

        /// <summary>
        /// Just like you do with controller methods, but for delegates.
        /// </summary>
        void MapRouteAction(RouteActionDelegate routeAction, RouteInfo routeInfo);

        IEnumerable<ActionDescriptor> GetRoutes();
    }


}
