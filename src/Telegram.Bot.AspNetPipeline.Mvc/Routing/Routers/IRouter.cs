using System.Threading.Tasks;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing.Routers
{
    /// <summary>
    /// Set handler for current RoteData if match.
    /// Or can edit route data. If Handler was not setted - will be handled by next router in list.
    /// <para></para>
    /// Please, use <see cref="BaseStringMatchRouter"/> in your implementions.
    /// </summary>
    public interface IRouter
    {
        Task RouteAsync(RoutingContext routeContext);
    }
}
