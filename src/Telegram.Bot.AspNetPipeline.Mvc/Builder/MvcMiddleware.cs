﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Exceptions;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.ModelBinding.Binders;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Services;
using Telegram.Bot.AspNetPipeline.Mvc.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Extensions;
using Telegram.Bot.AspNetPipeline.Mvc.Extensions.Main;
using Telegram.Bot.AspNetPipeline.Mvc.Extensions.MvcFeatures;
using Telegram.Bot.AspNetPipeline.Mvc.Routing;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Routers;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing.Implementions;

namespace Telegram.Bot.AspNetPipeline.Mvc.Builder
{
    public class MvcMiddleware : IMiddleware
    {
        readonly MainRouter _mainRouter;

        readonly MvcOptions _mvcOptions;

        #region Resolved services.
        readonly IContextPreparer _contextPreparer;

        readonly ServicesBus _servicesBus;

        readonly IGlobalSearchBag _globalSearchBag;
        #endregion

        /// <summary>
        /// All other services will be resolved from ServiceProvider.
        /// </summary>
        public MvcMiddleware(
            MvcOptions mvcOptions,
            IUseMvcBuilder useMvcBuilder
            )
        {
            useMvcBuilder.Routers = useMvcBuilder.Routers ?? new List<IRouter>();
            useMvcBuilder.ModelBinders = useMvcBuilder.ModelBinders ?? new List<IModelBinder>();

            _mvcOptions = (MvcOptions)mvcOptions.Clone();

            var serv = useMvcBuilder.ServiceProvider;
            _mainRouter = new MainRouter(useMvcBuilder.Routers);
            _contextPreparer = serv.GetRequiredService<IContextPreparer>();

            //Controllers.
            var controllers = useMvcBuilder.Controllers ?? new List<Type>();
            var startupRoutes = useMvcBuilder.GetRoutes();
            _globalSearchBag = CreateGlobalSearchBag(serv, startupRoutes, controllers);
            var mainModelBinder = new MainModelBinder(useMvcBuilder.ModelBinders);

            //Init services bus.
            _servicesBus = serv.GetRequiredService<ServicesBus>();
            var outerMiddlewaresInformer = new OuterMiddlewaresInformer(_mainRouter);
            var mvcFeatures = new MvcFeatures();
            _servicesBus.Init(
                _mainRouter,
                outerMiddlewaresInformer,
                mvcFeatures,
                mainModelBinder
                );            
        }

        /// <summary>
        /// Invoked before all UseMvc middlewares.
        /// </summary>
        public void SetGlobalSearchBag(UpdateContext ctx)
        {
            ctx.SetGlobalSearchBag(_globalSearchBag);
        }

        public async Task Invoke(UpdateContext ctx, Func<Task> next)
        {
            SetGlobalSearchBag(ctx);

            var routingCtx = new RoutingContext(ctx);
            await _mainRouter.RouteAsync(routingCtx);

            //Handler found.
            var actDesc = routingCtx.ActionDescriptor;
            if (actDesc != null)
            {
                var ctxLogger = ctx.Logger();
                if (ctxLogger.IsEnabled(LogLevel.Trace))
                {
                    ctxLogger
                        .LogTrace(
                            $"Mvc start processing context with '{actDesc.RouteInfo.Template ?? actDesc.RouteInfo.Name}'."
                            );
                }

                var actionContext = _contextPreparer.CreateContext(ctx, actDesc);
                await actDesc.Handler.Invoke(actionContext);

                await InvokeActionByName(actionContext, 1);
            }
            await next();
        }

        async Task InvokeActionByName(ActionContext prevActionContext, int invokesCount)
        {
            var max = _mvcOptions.StartAnotherActionMaxStackLevel;
            if (invokesCount > max)
                throw new TelegramAspException($"StartAnotherAction invoked recursive more than {max} times. " +
                                    $"You can change max value in MvcOptions.");

            var startAnotherActionData = MvcFeatures.GetData(prevActionContext);
            if (startAnotherActionData == null)
                return;
            var actName = startAnotherActionData.ActionName;

            var ctx = prevActionContext.UpdateContext;
            var actDesc = _globalSearchBag.FindByName(actName);
            if (actDesc == null)
            {
                throw new TelegramAspException($"Can't find action with name '{actName}'.");
            }

            var actionContext = _contextPreparer.CreateContext(ctx, actDesc);
            await actDesc.Handler.Invoke(actionContext);

            //Check again.
            await InvokeActionByName(actionContext, invokesCount + 1);

        }

        IGlobalSearchBag CreateGlobalSearchBag(
            IServiceProvider serv,
            IEnumerable<ActionDescriptor> startupRoutes,
            IList<Type> controllers
            )
        {
            //Init routes (ActionDescriptors) search bag.

            //Smallest controllers code in MvcMiddleware class that i can write.
            var controllersInspector = serv.GetRequiredService<IControllerInspector>();
            var controllersRoutes = new List<ActionDescriptor>();
            foreach (var controllerType in controllers)
            {
                var routes = controllersInspector.Inspect(controllerType);
                controllersRoutes.AddRange(routes);
            }

            var allRoutes = controllersRoutes.ToList();
            allRoutes.AddRange(startupRoutes);
            var globalSearchBagProvider = serv.GetRequiredService<GlobalSearchBagProvider>();
            //Search bag initialized. 
            //All routes you can get only with IGlobalSearchBag.

            return globalSearchBagProvider.Resolve(allRoutes, _mvcOptions.CheckEqualsRouteInfo);
        }
    }
}
