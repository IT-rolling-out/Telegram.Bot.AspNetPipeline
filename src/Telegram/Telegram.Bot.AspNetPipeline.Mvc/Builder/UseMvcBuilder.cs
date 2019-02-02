using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.AspNetPipeline.Mvc.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing;

namespace Telegram.Bot.AspNetPipeline.Mvc.Builder
{
    public class UseMvcBuilder : IUseMvcBuilder
    {
        readonly IList<Tuple<RouteActionDelegate, RouteInfo>> _routeAction=new List<Tuple<RouteActionDelegate, RouteInfo>>();

        public UseMvcBuilder(IList<IRouter> routers)
        {
            Routers = routers;
        }

        public IList<IRouter> Routers { get; }

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
            _routeAction.Add(tuple);
        }

        public IEnumerable<Tuple<RouteActionDelegate, RouteInfo>> GetRouteActions()
        {
            return _routeAction.ToList();
        }
    }


}
