using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.Services.Implementions
{
    public class ControllerActionPreparer : IControllerActionPreparer
    {
        /// <summary>
        /// Create action from controller data.
        /// <para></para>
        /// All controller logic (like creating controllers, ModelBinders etc.) must be here.
        /// To know why - read readme.txt in Telegram.Bot.AspNetPipeline.Mvc.Controllers folder.
        /// <para></para>
        /// Current implemention use only <see cref="ControllerActionContext"/>, but you can use passed parameters
        /// to generate some predefined code to make it faster.
        /// </summary>
        public RouteActionDelegate CreateAction(Type controllerType, MethodInfo methodInfo, RouteInfo routeInfo)
        {
            RouteActionDelegate routeActionDelegate = async (actionContext) =>
             {
#if DEBUG
                 if (actionContext is ControllerActionContext controllerActionContext)
                 {
                     if (controllerActionContext.ActionDescriptor.MethodInfo != methodInfo)
                     {
                         throw new Exception("Predefined MethodInfo and context MethodInfo is different.");
                     }
                 }
#endif
                 if (!(actionContext is ControllerActionContext))
                 {
                     throw new Exception(
                         $"Controller actions can be invoked only with {typeof(ControllerActionContext).Name}.\n" +
                         $"Please, contact library author if exception throwed in default pipeline."
                     );
                 }
                 await Handler((ControllerActionContext)actionContext);
             };
            return routeActionDelegate;
        }

        public static async Task Handler(ControllerActionContext controllerActionContext)
        {
            var serv = controllerActionContext.UpdateContext.Services;
            var factory = serv.GetService<IControllersFactory>();
            BotController controller = factory.Create(controllerActionContext);
            BotController.InvokeInitializer(controller, controllerActionContext);

            var methodInfo = controllerActionContext.ActionDescriptor.MethodInfo;

            //TODO model binders create parameters, invoke methodinfo.
        }
    }
}
