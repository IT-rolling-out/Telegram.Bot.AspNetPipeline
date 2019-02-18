using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Extensions.MvcFeatures
{
    /// <summary>
    /// Some other mvc features.
    /// <para></para>
    /// Can't be resolved with IOC.
    /// </summary>
    public interface IMvcFeatures
    {
        /// <summary>
        /// Started another controller action when current action execution finished.
        /// Just pass current <see cref="UpdateContext"/> (not <see cref="ActionContext"/>, it will be created for new action) to another action.
        /// <para></para>
        /// Will not be executed if <see cref="UpdateContext.ForceExit"/> was invoked.
        /// </summary>
        /// <param name="name">Name property from BotRouteAttribute.</param>
        void StartAnotherAction(ActionContext actionContext, string name);
    }
}
