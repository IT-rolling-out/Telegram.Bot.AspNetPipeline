using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.AspNetPipeline.Mvc.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Routers;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing;

namespace Telegram.Bot.AspNetPipeline.Mvc.Builder
{
    internal class UseMvcBuilder : IUseMvcBuilder
    {
        readonly IList<RouteDescriptionData> _routeDescriptions=new List<RouteDescriptionData>();

        public IList<IRouter> Routers { get; } = new List<IRouter>();

        /// <summary>
        /// Just like you do with controller methods, but for delegates.
        /// </summary>
        public void MapRouteAction(RouteActionDelegate routeAction, RouteInfo routeInfo)
        {
            if (routeAction == null)
                throw new ArgumentNullException(nameof(routeAction));
            if (routeInfo == null)
                throw new ArgumentNullException(nameof(routeInfo));
            var tuple = new Tuple<RouteActionDelegate, RouteInfo>(routeAction, routeInfo);
            _routeDescriptions.Add(new RouteDescriptionData(routeAction, routeInfo));

        }

        public IEnumerable<RouteDescriptionData> GetRouteActions()
        {
            return _routeDescriptions.ToList();
        }
    }


}
