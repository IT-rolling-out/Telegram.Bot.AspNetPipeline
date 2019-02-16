using Telegram.Bot.AspNetPipeline.Mvc.Extensions;
using Telegram.Bot.AspNetPipeline.Mvc.Extensions.Main;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Routers;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing
{
    /// <summary>
    /// Implemented with <see cref="ServicesBus"/>.
    /// </summary>
    public interface IMainRouterProvider
    {
        MainRouter MainRouter { get; }
    }
}
