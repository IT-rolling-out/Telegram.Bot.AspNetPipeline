using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Core
{
    /// <summary>
    /// NOTE: All <see cref="RouteActionDelegate"/>s from controller methods throw exception
    /// if was passed not <see cref="ControllerActionContext"/>.
    /// </summary>
    public delegate Task RouteActionDelegate(ActionContext actionContext);
}
