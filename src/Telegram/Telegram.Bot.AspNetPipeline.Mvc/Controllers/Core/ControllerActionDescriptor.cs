using System;
using System.Reflection;
using Telegram.Bot.AspNetPipeline.Mvc.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core
{
    /// <summary>
    /// Static data about registered method or method.
    /// <para></para>
    /// Controller routes top object with static data.
    /// </summary>
    public class ControllerActionDescriptor : ActionDescriptor
    {
        public ControllerActionDescriptor(
            RouteActionDelegate handler,
            RouteInfo routeInfo,
            MethodInfo methodInfo,
            Type controllerType
            ) : base(handler,routeInfo)
        {
            MethodInfo = methodInfo;
            ControllerType = controllerType;
        }

        public MethodInfo MethodInfo { get; }

        public Type ControllerType { get; }

    }
}
