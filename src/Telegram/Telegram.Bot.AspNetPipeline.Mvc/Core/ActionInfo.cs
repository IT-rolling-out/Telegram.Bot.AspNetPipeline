﻿using Telegram.Bot.AspNetPipeline.Mvc.Routing;

namespace Telegram.Bot.AspNetPipeline.Mvc.Core
{
    /// <summary>
    /// Static data about registered method or delegate.
    /// </summary>
    public class ActionInfo
    {
        public ActionInfo(RouteInfo routeInfo)
        {
            RouteInfo = routeInfo;
        }

        public RouteInfo RouteInfo { get; }
    }
}
