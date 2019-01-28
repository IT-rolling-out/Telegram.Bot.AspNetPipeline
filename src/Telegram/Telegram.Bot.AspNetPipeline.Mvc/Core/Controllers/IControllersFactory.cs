namespace Telegram.Bot.AspNetPipeline.Mvc.Core.Controllers
{
    public interface IControllersFactory
    {
        BotController Create(ControllerActionInfo controllerActionInfo);
    }
}
