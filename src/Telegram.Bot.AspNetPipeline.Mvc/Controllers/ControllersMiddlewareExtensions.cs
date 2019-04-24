using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Mvc.Builder;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.ModelBinding;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.ModelBinding.Binders;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Services;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Services.Implementions;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers
{
    internal static class ControllersMiddlewareExtensions
    {
        public static void AddControllersServices(IAddMvcBuilder builder)
        {
            var serv = builder.ServiceCollection;
            serv.AddSingleton<IControllersFactory, ControllersFactory>();
            serv.AddSingleton<IControllerInspector, ControllerInspector>();
            serv.AddSingleton<IControllerActionPreparer, ControllerActionPreparer>();
            serv.AddSingleton<IControllersFactory, ControllersFactory>();

            //Register controllers.
            foreach (var controllerType in builder.Controllers)
            {
                serv.AddTransient(controllerType);
            }
        }

        public static void InitAddMvcBuilder(MvcOptions options, IAddMvcBuilder builder)
        {
            if (options.FindControllersByReflection)
            {
                //Search controllers.
                builder.Controllers = ControllersTypesSearch.FindAllControllers();
            }
        }

        public static void InitUseMvcBuilder(IUseMvcBuilder builder)
        {
            //Add Model binders.
            builder.ModelBinders = new List<IModelBinder>
            {
                new SpacingModelBinder()
            };
        }

        
    }
}
