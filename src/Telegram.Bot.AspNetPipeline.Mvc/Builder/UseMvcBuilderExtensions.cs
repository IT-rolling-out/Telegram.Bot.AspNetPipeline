using System;
using System.Collections.Generic;
using IRO.Common.Collections;
using Telegram.Bot.AspNetPipeline.Mvc.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.AspNetPipeline.Mvc.Builder
{
    public static class UseMvcBuilderExtensions
    {
        public static void MapRouteAction(
            this IUseMvcBuilder @this,
            RouteActionDelegate routeAction,
            string template = null,
            int order = 0,
            string name = null,
            UpdateType[] updateTypes = null
        )
        {
            var routeInfo = new RouteInfo(template, order, name, updateTypes);
            @this.MapRouteAction(routeAction, routeInfo);
        }

        public static void RemoveControllersByNamespace(
            this IUseMvcBuilder @this, 
            string namespaceToRemove
        )
        {
            if (namespaceToRemove == null) 
                throw new ArgumentNullException(nameof(namespaceToRemove));
            @this.Controllers = @this.Controllers?? new List<Type>();
            var filteredControllers = new List<Type>();
            foreach (var type in @this.Controllers)
            {
                if (!type.Namespace.StartsWith(namespaceToRemove))
                {
                    filteredControllers.Add(type);
                }
            }
            @this.Controllers = filteredControllers;
        }
    }
}
