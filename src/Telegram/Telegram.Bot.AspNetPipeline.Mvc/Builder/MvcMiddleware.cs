using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Services;
using Telegram.Bot.AspNetPipeline.Mvc.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Core.Services;
using Telegram.Bot.AspNetPipeline.Mvc.Extensions;
using Telegram.Bot.AspNetPipeline.Mvc.Extensions.Main;
using Telegram.Bot.AspNetPipeline.Mvc.Routing;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Routers;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing.Implementions;

namespace Telegram.Bot.AspNetPipeline.Mvc.Builder
{
    public class MvcMiddleware : IMiddleware
    {
        readonly MainRouter _mainRouter;

        #region Resolved services.
        readonly IContextPreparer _contextPreparer;

        readonly ServicesBus _servicesBus;

        readonly IGlobalSearchBag _globalSearchBag;
        #endregion

        /// <summary>
        /// All other services will be resolved from ServiceProvider.
        /// </summary>
        public MvcMiddleware(IAddMvcBuilder addMvcBuilder, IUseMvcBuilder useMvcBuilder)
        {
            var serv = useMvcBuilder.ServiceProvider;
            _mainRouter = new MainRouter(useMvcBuilder.Routers);
            _contextPreparer = serv.GetService<IContextPreparer>();

            var controllers = addMvcBuilder.Controllers;
            var startupRoutes = useMvcBuilder.GetRoutes();
            _globalSearchBag = InitGlobalSearchBagProvider(serv, startupRoutes, controllers);

            //Init services bus.
            _servicesBus = serv.GetService<ServicesBus>();
            var outerMiddlewaresInformer = new OuterMiddlewaresInformer(_mainRouter);
            var mvcFeatures = new MvcFeatures();
            _servicesBus.Init(
                _mainRouter, 
                outerMiddlewaresInformer, 
                mvcFeatures
                );

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

                await InvokeActionByName(actionContext);
            }
            await next();
        }

        async Task InvokeActionByName(ActionContext prevActionContext)
        {
            var startAnotherActionData = MvcFeatures.GetData(prevActionContext);
            if (startAnotherActionData == null)
                return;

            var ctx = prevActionContext.UpdateContext;
            var actDesc = _globalSearchBag.FindByName(startAnotherActionData.ActionName);
            if (actDesc != null)
            {
                var actionContext = _contextPreparer.CreateContext(ctx, actDesc);
                await actDesc.Handler.Invoke(actionContext);

                //Check again.
                await InvokeActionByName(actionContext);
            }
        }

        IGlobalSearchBag InitGlobalSearchBagProvider(
            IServiceProvider serv, 
            IEnumerable<ActionDescriptor> startupRoutes, 
            IList<Type> controllers
            )
        {
            //Init routes (ActionDescriptors) search bag.

            //Smallest controllers code in MvcMiddleware class that i can write.
            var controllersInspector = serv.GetService<IControllerInpector>();
            var controllersRoutes = new List<ActionDescriptor>();
            foreach (var controllerType in controllers)
            {
                var routes = controllersInspector.Inspect(controllerType);
                controllersRoutes.AddRange(routes);
            }

            var allRoutes = controllersRoutes.ToList();
            allRoutes.AddRange(startupRoutes);
            var globalSearchBagProvider = serv.GetService<GlobalSearchBagProvider>();
            globalSearchBagProvider.Init(allRoutes);
            //Search bag initialized. 
            //All routes you can get only with IGlobalSearchBag.

            return globalSearchBagProvider.Resolve();
        }
    }
}
