using System;
using System.Reflection;
using Telegram.Bot.AspNetPipeline.Mvc.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core
{
    public class ControllerActionDescriptor:ActionDescriptor
    {
        public ControllerActionDescriptor(MethodInfo methodInfo, Type controllerType, RouteInfo routeInfo):base(routeInfo)
        {
            MethodInfo = methodInfo;
            ControllerType = controllerType;
        }

        public MethodInfo MethodInfo { get; }

        public Type ControllerType { get; }

    }
}
