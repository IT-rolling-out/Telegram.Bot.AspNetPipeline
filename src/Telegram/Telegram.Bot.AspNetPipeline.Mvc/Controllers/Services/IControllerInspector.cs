using System;
using System.Collections.Generic;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.Services
{
    /// <summary>
    /// Main object to create controller actions.
    /// <para></para>
    /// Can be overrided with IOC.
    /// </summary>
    public interface IControllerInspector
    {
        IEnumerable<ControllerActionDescriptor> Inspect(Type controllerType);
    }
}