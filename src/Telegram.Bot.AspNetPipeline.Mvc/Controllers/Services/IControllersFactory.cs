using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.Services
{
    /// <summary>
    /// Can be overrided in IOC.
    /// </summary>
    public interface IControllersFactory
    {
        BotController Create(ControllerActionContext controllerActionContext);
    }
}
