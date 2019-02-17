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
        readonly IList<ActionDescriptor> _routes = new List<ActionDescriptor>();

        public IServiceProvider ServiceProvider { get; }

        public IList<IRouter> Routers { get; } = new List<IRouter>()
        {
            new FullMatchRouter()
        };

        public UseMvcBuilder(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

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
            _routes.Add(new ActionDescriptor(routeAction, routeInfo));

        }

        public IEnumerable<ActionDescriptor> GetRoutes()
        {
            return _routes.ToList();
        }
    }


}
