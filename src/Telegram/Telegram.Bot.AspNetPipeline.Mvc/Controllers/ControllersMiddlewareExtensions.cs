using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Mvc.Builder;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.ModelBinding;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Services;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Services.Implementions;
using Telegram.Bot.AspNetPipeline.Mvc.Core.Services;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers
{
    public static class ControllersMiddlewareExtensions
    {
        public static void AddControllersServices(IAddMvcBuilder builder)
        {
            var serv = builder.ServiceCollection;
            serv.AddSingleton<IControllersFactory, ControllersFactory>();
            serv.AddSingleton<IControllerInpector, ControllerInpector>();
            serv.AddSingleton<IControllerActionPreparer, ControllerActionPreparer>();
            serv.AddSingleton<IControllersFactory, ControllersFactory>();

            //Register controllers.
            foreach (var controllerType in builder.Controllers)
            {
                serv.AddTransient(controllerType);
            }
            
            //TODO Add Model binders.
            
        }

        public static void InitAddMvcBuilder(MvcOptions options, IAddMvcBuilder builder)
        {
            IList<Type> controllers = null;
            if (options.FindControllersByReflection)
            {
                //Search controllers.
                controllers = ControllersTypesSearch.FindAllControllers();
            }
            if (controllers != null)
            {
                builder.Controllers = controllers;
            }

            builder.ModelBinders = new List<Type>
            {

            };



        }
    }
}
