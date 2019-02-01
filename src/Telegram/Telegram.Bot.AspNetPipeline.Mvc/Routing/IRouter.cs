using System.Threading.Tasks;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing
{
    /// <summary>
    /// Set handler for current RoteData if match.
    /// Or can edit rote data. If Handler was not setted - will be handled by next router in list.
    /// </summary>
    public interface IRouter
    {
        /// <summary>
        /// Can only set TemplateMatchingStrings that used in searching of handler.
        /// </summary>
        Task ProcessRouteContext(RoutingContext routeContext);
    }
}
