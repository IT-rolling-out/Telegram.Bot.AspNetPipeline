using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Exceptions;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.ModelBinding;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.ModelBinding.Binders;
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
            if (!typeof(Task).IsAssignableFrom(methodInfo.ReturnType))
            {
                throw new TelegramAspException($"Exception with '{methodInfo}' in '{controllerType.Name}'. " +
                                    $"Controller method must return Task.");
            }

            RouteActionDelegate routeActionDelegate = async (actionContext) =>
             {
#if DEBUG
                 if (actionContext is ControllerActionContext controllerActionContext)
                 {
                     if (controllerActionContext.ActionDescriptor.MethodInfo != methodInfo)
                     {
                         throw new TelegramAspException("Predefined MethodInfo and context MethodInfo is different.");
                     }
                 }
#endif
                 if (!(actionContext is ControllerActionContext))
                 {
                     throw new TelegramAspException(
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
            var factory = serv.GetRequiredService<IControllersFactory>();
            BotController controller = factory.Create(controllerActionContext);
            BotController.InvokeInitializer(controller, controllerActionContext);
            var methodInfo = controllerActionContext.ActionDescriptor.MethodInfo;

            //Model binding.
            var modelBindingContext = new ModelBindingContext(controllerActionContext);
            var mainModelBinder = serv
                .GetRequiredService<IMainModelBinderProvider>().MainModelBinder;
            await mainModelBinder.Bind(modelBindingContext);
            controllerActionContext.IsModelStateValid = modelBindingContext.IsAllBinded();
            var invokationParams = modelBindingContext.ToMethodParameters();

            //Invoke.
            var methodResult=methodInfo.Invoke(controller, invokationParams);
            if (methodResult is Task task)
            {
                await task;
            }
        }
    }
}
