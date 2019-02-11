using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.MiddlewareServices
{
    public interface IControllersFactory
    {
        BotController Create(ControllerActionDescriptor controllerActionDescriptor);
    }
}
