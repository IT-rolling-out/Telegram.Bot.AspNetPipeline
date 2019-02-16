using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.Services
{
    public interface IControllersFactory
    {
        BotController Create(ControllerActionContext controllerActionContext);
    }
}
