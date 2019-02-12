﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.MiddlewareServices;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.MiddlewareServices.Implementions;
using Telegram.Bot.AspNetPipeline.Mvc.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Routers;

namespace Telegram.Bot.AspNetPipeline.Mvc.Builder
{
    public class MvcMiddleware : IMiddleware
    {
        readonly MainRouter _mainRouter;

        #region Resolved services.
        IControllerInpector _controllerInpector;

        IContextPreparer _contextPreparer;
        #endregion

        /// <summary>
        /// All other services will be resolved from ServiceProvider.
        /// </summary>
        public MvcMiddleware(IAddMvcBuilder addMvcBuilder, IUseMvcBuilder useMvcBuilder)
        {
            _mainRouter = new MainRouter(useMvcBuilder.Routers);
        }

        public async Task Invoke(UpdateContext ctx, Func<Task> next)
        {
            var routingCtx = new RoutingContext(ctx);
            await _mainRouter.RouteAsync(routingCtx);

            //Handler found.
            var actDesc = routingCtx.ActionDescriptor;
            if (actDesc != null)
            {
                var actionContext = _contextPreparer.CreateContext(ctx, actDesc);
                await actDesc.Handler.Invoke(actionContext);
            }
            await next();
        }

        void ResolveServices(IServiceProvider serviceProvider)
        {
            _controllerInpector = serviceProvider.GetService<IControllerInpector>();
            _contextPreparer = serviceProvider.GetService<IContextPreparer>();
        }

        #region Register services.
        /// <summary>
        /// I think it must be here, not in extensions class.
        /// <para></para>
        /// Static, because MvcMiddleware created after DI container builded, when we can't add new services.
        /// </summary>
        internal static void RegisterServices(
            ServiceCollectionWrapper serviceCollectionWrapper,
            MvcOptions mvcOptions = null,
            Action<IAddMvcBuilder> configureAddMvcBuilder = null)
        {
            var serv = serviceCollectionWrapper.Services;
            //First step. Register all services, that can be registered before IAddMvcBuilder configurations.
            RegisterServises_NotRequiredBuilder(serviceCollectionWrapper);

            //Second step. Init AddMvcBuilder parameters with MvcOptions to pass it to callback.
            mvcOptions = mvcOptions ?? new MvcOptions();
            var addMvcBuilder = InitAddMvcBuilder(serviceCollectionWrapper, mvcOptions);

            //Custom configs.
            configureAddMvcBuilder?.Invoke(addMvcBuilder);

            //Third step. Register all services based on IAddMvcBuilder.
            RegisterServises_RequiredBuilder(
                serviceCollectionWrapper,
                addMvcBuilder
                );

            //Finish. Register builders to be resolved in middleware.
            serv.AddSingleton<IAddMvcBuilder>(addMvcBuilder);
            serv.AddSingleton<IUseMvcBuilder, UseMvcBuilder>();
        }

        static void RegisterServises_NotRequiredBuilder(ServiceCollectionWrapper serviceCollectionWrapper)
        {
            var serv = serviceCollectionWrapper.Services;
            serviceCollectionWrapper.AddControllersFactory<ControllersFactory>();
            serv.AddSingleton<ControllerInpector>();
        }

        static void RegisterServises_RequiredBuilder(ServiceCollectionWrapper serviceCollectionWrapper, IAddMvcBuilder addMvcBuilder)
        {
            var serv = serviceCollectionWrapper.Services;
            foreach (var controllerType in addMvcBuilder.Controllers)
            {
                serv.AddTransient(controllerType);
            }

        }

        static IAddMvcBuilder InitAddMvcBuilder(ServiceCollectionWrapper serviceCollectionWrapper, MvcOptions mvcOptions)
        {
            var serv = serviceCollectionWrapper.Services;
            IList<Type> controllers = null;
            if (mvcOptions.FindControllersByReflection)
            {
                //Search controllers.
                controllers = ControllersTypesSearch.FindAllControllers();
            }
            controllers = controllers ?? new List<Type>();
            IAddMvcBuilder addMvcBuilder = new AddMvcBuilder(
                mvcOptions,
                controllers,
                serv
                );
            return addMvcBuilder;
        }
        #endregion
    }
}
