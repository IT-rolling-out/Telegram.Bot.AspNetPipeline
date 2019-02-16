using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Extensions.Main
{
    /// <summary>
    /// Inject IOuterMiddlewaresInformerProvider with DI to your service and use.
    /// Ignore some telegram messages, if mvc has bigger priority.
    /// </summary>
    public interface IOuterMiddlewaresInformer
    {
        /// <summary>
        /// If mvc has method with bigger priority - return true.
        /// <para></para>
        /// Lower number mean bigger priority.
        /// </summary>
        Task<bool> CheckMvcHasPriorityHandler(UpdateContext updateContext, int yourMethodOrder);
    }
}

