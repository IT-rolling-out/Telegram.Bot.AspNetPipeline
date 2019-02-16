using System;
using System.Collections.Generic;
using System.Reflection;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Metadata;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.Services.Implementions
{
    /// <summary>
    /// Main object to create controller actions.
    /// </summary>
    public class ControllerInpector : IControllerInpector
    {
        readonly IControllerActionPreparer _controllerActionPreparer;

        public ControllerInpector(IControllerActionPreparer controllerActionPreparer)
        {
            _controllerActionPreparer = controllerActionPreparer;
        }

        /// <summary>
        /// Invoked on each controller method (with BotRouteAttribute) with controller, MethodInfo, action info.
        /// </summary>
        public event ControllerInspectDelegate ProcessingMethodInfo;

        /// <summary>
        /// Inspect controller methods with reflection and resolve ControllerActionDescriptor.
        /// </summary>
        public IEnumerable<ControllerActionDescriptor> Inspect(Type controllerType)
        {
            if (!controllerType.IsInstanceOfType(typeof(BotController)))
            {
                throw new Exception($"Type '{controllerType}' is not instance of type '{typeof(BotController)}'");
            }
            if (controllerType.IsAbstract)
            {
                throw new Exception($"Controller type '{controllerType}' can't be abstract.");
            }
            if (controllerType.IsGenericTypeDefinition)
            {
                throw new Exception($"Controller type '{controllerType}' can't be Generic definitions.");
            }

            var resList = new List<ControllerActionDescriptor>();
            foreach (var method in controllerType.GetMethods())
            {
                var arr = method.GetCustomAttributes(typeof(BotRouteAttribute), true);
                if (arr.Length > 1)
                {
                    throw new Exception($"Method '{controllerType.Name}.{method.Name}' has multiple BotRouteAttributes.");
                }

                if (arr.Length == 1)
                {
                    //Found controller action method.
                    var botRouteAttribute = (BotRouteAttribute)arr[0];
                    var controllerActionDescriptor= ProcessMethodInfo(controllerType, method, botRouteAttribute);
                    resList.Add(controllerActionDescriptor);
                }
            }

            return resList;
        }

        ControllerActionDescriptor ProcessMethodInfo(Type controllerType, MethodInfo methodInfo, BotRouteAttribute routeAttr)
        {
            var routeInfo = new RouteInfo(
                routeAttr.Template,
                routeAttr.Order,
                routeAttr.Name,
                routeAttr.UpdateTypes
                );

            var handler = _controllerActionPreparer.CreateAction(controllerType, methodInfo, routeInfo);

            var controllerActionDescriptor = new ControllerActionDescriptor(handler, routeInfo, methodInfo, controllerType);
            ProcessingMethodInfo?.Invoke(controllerActionDescriptor);
            return controllerActionDescriptor;
        }
    }
}
