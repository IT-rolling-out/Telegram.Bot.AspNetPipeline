using System;
using System.Reflection;
using Telegram.Bot.AspNetPipeline.Mvc.Core.Routing;

namespace Telegram.Bot.AspNetPipeline.Mvc.Core.Controllers
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
