using System;
using System.Collections.Generic;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.MiddlewareServices
{
    /// <summary>
    /// Main object to create controller actions.
    /// </summary>
    public interface IControllerInpector
    {
        IEnumerable<ControllerActionDescriptor> Inspect(Type controllerType);
    }
}