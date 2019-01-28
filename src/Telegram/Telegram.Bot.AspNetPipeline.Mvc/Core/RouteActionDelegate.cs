using System.Threading.Tasks;

namespace Telegram.Bot.AspNetPipeline.Mvc.Core
{
    public delegate Task RouteActionDelegate(ActionContext actionContext);
}
