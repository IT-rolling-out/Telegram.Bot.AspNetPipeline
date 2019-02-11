using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.MiddlewareServices
{
    public delegate void ControllerInspectDelegate(
        ControllerActionDescriptor controllerActionDescriptor,
        RouteActionDelegate routeActionDelegate
    );
}
