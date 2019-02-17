using System;
using System.Collections.Generic;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.ModelBinding;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.ModelBinding.Binders;
using Telegram.Bot.AspNetPipeline.Mvc.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Routers;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing;

namespace Telegram.Bot.AspNetPipeline.Mvc.Builder
{
    /// <summary>
    /// Not added to DI conteiner.
    /// </summary>
    public interface IUseMvcBuilder
    {
        IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Default routers.  
        /// </summary>
        IList<IRouter> Routers { get; set; }

        /// <summary>
        /// Types must implement <see cref="IModelBinder"/>. Register type to resolve it with ioc.
        /// <para></para>
        /// Without ModelBinderProvider and more simpler, like it was in asp.net.
        /// But you can build your own asp-like model binding middleware, if needed.
        /// </summary>
        IList<IModelBinder> ModelBinders { get; set; }

        /// <summary>
        /// Just like you do with controller methods, but for delegates.
        /// </summary>
        void MapRouteAction(RouteActionDelegate routeAction, RouteInfo routeInfo);

        IEnumerable<ActionDescriptor> GetRoutes();
    }


}
