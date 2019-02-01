using System;
using System.Reflection;
using Telegram.Bot.AspNetPipeline.Mvc.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers
{
    public class ControllerActionInfo:ActionInfo
    {
        public ControllerActionInfo(MethodInfo methodInfo, Type controllerType, RouteInfo routeInfo):base(routeInfo)
        {
            MethodInfo = methodInfo;
            ControllerType = controllerType;
        }

        public MethodInfo MethodInfo { get; }

        public Type ControllerType { get; }

    }
}
