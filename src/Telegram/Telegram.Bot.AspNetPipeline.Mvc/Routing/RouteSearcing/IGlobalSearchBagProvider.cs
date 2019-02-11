﻿using System.Collections.Generic;
using Telegram.Bot.AspNetPipeline.Mvc.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing
{
    /// <summary>
    /// Can be used with DI.
    /// </summary>
    public interface IGlobalSearchBagProvider
    {
        /// <summary>
        /// Called once.
        /// </summary>
        void Init(IEnumerable<RouteDescriptionData> routeDescriptions);
       
        IGlobalSearchBag Resolve();
    }
}
