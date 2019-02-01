namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers
{
    public interface IControllersFactory
    {
        BotController Create(ControllerActionInfo controllerActionInfo);
    }
}
