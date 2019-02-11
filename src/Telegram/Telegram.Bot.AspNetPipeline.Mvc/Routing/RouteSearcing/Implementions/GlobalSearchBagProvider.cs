using System;
using System.Collections.Generic;
using Telegram.Bot.AspNetPipeline.Mvc.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing.Implementions
{
    public class GlobalSearchBagProvider : IGlobalSearchBagProvider
    {
        GlobalSearchBag _globalSearchBagSingleton;

        public void Init(IEnumerable<RouteDescriptionData> routeDescriptions)
        {
            routeDescriptions = routeDescriptions ?? throw new ArgumentNullException(nameof(routeDescriptions));
            _globalSearchBagSingleton = new GlobalSearchBag(routeDescriptions);
        }

        public IGlobalSearchBag Resolve()
        {
            return _globalSearchBagSingleton;
        }
    }
}
