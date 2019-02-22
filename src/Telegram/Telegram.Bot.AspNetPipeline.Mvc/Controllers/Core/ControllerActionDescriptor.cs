using System;
using System.Collections.Generic;
using System.Reflection;
using Telegram.Bot.AspNetPipeline.Mvc.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core
{
    /// <summary>
    /// Static data about registered method.
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
            Parameters=methodInfo.GetParameters();
            foreach (var param in Parameters)
            {
                _parametersByName[param.Name] = param;
            }
        }

        IDictionary<string, ParameterInfo> _parametersByName = new Dictionary<string, ParameterInfo>();

        public MethodInfo MethodInfo { get; }

        public ParameterInfo[] Parameters { get; }

        public Type ControllerType { get; }

        /// <summary>
        /// Optimized parameters searching.
        /// </summary>
        public ParameterInfo FindParameter(string paramName)
        {
            return _parametersByName[paramName];
        }

    }
}
