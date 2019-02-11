using System;
using System.Collections.Generic;
using System.Reflection;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Metadata;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.MiddlewareServices
{
    public class ControllerInpector
    {
        /// <summary>
        /// Invoked on each controller method (with BotRouteAttribute) with controller, MethodInfo, action info.
        /// </summary>
        public event ControllerInspectDelegate ProcessingMethodInfo;

        /// <summary>
        /// Inspect controller methods with reflection and resolve ControllerActionDescriptor.
        /// </summary>
        public IEnumerable<RouteDescriptionData> Inspect(Type controllerType)
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
                    ProcessMethod(controllerType, method, botRouteAttribute);
                }
            }

            throw new NotImplementedException();
        }

        void ProcessMethod(Type controllerType, MethodInfo methodInfo, BotRouteAttribute routeAttr)
        {
            var routeInfo = new RouteInfo(
                routeAttr.Template,
                routeAttr.Order,
                routeAttr.Name,
                routeAttr.UpdateTypes
                );

            var controllerActionDescriptor = new ControllerActionDescriptor(methodInfo, controllerType, routeInfo);
            //var routeDescription = new RouteDescriptionData(handler, routeInfo);
            //ProcessingMethodInfo?.Invoke();
        }


    }
}
