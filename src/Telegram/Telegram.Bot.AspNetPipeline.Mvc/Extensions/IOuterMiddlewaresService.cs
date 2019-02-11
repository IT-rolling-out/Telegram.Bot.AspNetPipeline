using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Mvc.Routing;

namespace Telegram.Bot.AspNetPipeline.Mvc.Extensions
{
    /// <summary>
    /// Inject it with DI to your service and use.
    /// Ignore some telegram messages, if mvc has bigger priority.
    /// </summary>
    public interface IOuterMiddlewaresService
    {
        /// <summary>
        /// If mvc has method with bigger priority - return true.
        /// </summary>
        Task<bool> CheckMvcHasPriorityHandler(RouteInfo routeInfo);

        /// <summary>
        /// If mvc has method with bigger priority - return true.
        /// <para></para>
        /// Lower number mean bigger priority.
        /// </summary>
        Task<bool> CheckMvcHasPriorityHandler(int yourMethodOrder);
    }
}

