using System;
using System.Reflection;
using Telegram.Bot.AspNetPipeline.Mvc.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.Services
{
    /// <summary>
    /// Used in default ControllerInspector.
    /// <para></para>
    /// Main object to add controllers-special services.
    /// </summary>
    public interface IControllerActionPreparer
    {
        /// <summary>
        /// Create action from controller data.
        /// <para></para>
        /// All controller logic (like creating controllers, ModelBinders etc.) must be here.
        /// To know why - read readme.txt in Telegram.Bot.AspNetPipeline.Mvc.Controllers folder.
        /// </summary>
        RouteActionDelegate CreateAction(
            Type controllerType,
            MethodInfo methodInfo,
            RouteInfo routeInfo
            );
    }
}
