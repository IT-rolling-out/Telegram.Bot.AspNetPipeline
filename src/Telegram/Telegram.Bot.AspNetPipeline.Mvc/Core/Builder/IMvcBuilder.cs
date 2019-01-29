using System;
using System.Collections.Generic;
using Telegram.Bot.AspNetPipeline.Mvc.Core.Controllers;
using Telegram.Bot.AspNetPipeline.Mvc.Core.Routing;

namespace Telegram.Bot.AspNetPipeline.Mvc.Core.Builder
{
    public interface IMvcBuilder
    {
        IList<IRouter> Routers { get; }

        /// <summary>
        /// Convert ControllerActionInfo to RouteActionDelegate.
        /// Default IControllerPreparer on each call create controller using IOC and invoke MethodInfo.
        /// <para></para>
        /// You can override it  and add what you want, for example, model binding.
        /// </summary>
        IControllerActionPreparer ControllerActionPreparer { get; set; }

        /// <summary>
        /// Default factory use ioc to resolve controllers.
        /// </summary>
        IControllersFactory ControllersFactory { get; set; }

        /// <summary>
        /// Just like you do with controller methods, but for delegates.
        /// </summary>
        void MapRouteAction(RouteActionDelegate routeAction, RouteInfo routeInfo);
    }
}
