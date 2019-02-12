using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.MiddlewareServices.Implementions
{
    public delegate void ControllerInspectDelegate(
        ControllerActionDescriptor controllerActionDescriptor
    );
}
