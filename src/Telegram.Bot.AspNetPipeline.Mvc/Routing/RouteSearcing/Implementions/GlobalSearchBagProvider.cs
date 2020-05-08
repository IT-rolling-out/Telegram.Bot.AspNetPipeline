using System;
using System.Collections.Generic;
using Telegram.Bot.AspNetPipeline.Exceptions;
using Telegram.Bot.AspNetPipeline.Mvc.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing.Implementions
{
    internal class GlobalSearchBagProvider : IGlobalSearchBagProvider
    {
        public IGlobalSearchBag Resolve(IEnumerable<ActionDescriptor> routeDescriptions, bool checkEqualsRouteInfo)
        {
            return new GlobalSearchBag(routeDescriptions, checkEqualsRouteInfo); ;
        }
    }
}
