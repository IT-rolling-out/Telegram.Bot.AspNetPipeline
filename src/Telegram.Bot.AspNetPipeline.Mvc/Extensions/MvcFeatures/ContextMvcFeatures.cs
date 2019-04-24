using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Extensions.MvcFeatures
{
    public class ContextMvcFeatures
    {
        readonly IMvcFeatures _mvcFeatures;

        readonly ActionContext _ctx;

        public ContextMvcFeatures(IMvcFeatures mvcFeatures, ActionContext actionContext)
        {
            _mvcFeatures = mvcFeatures;
            _ctx = actionContext;
        }

        /// <summary>
        /// Started another controller action when current action execution finished.
        /// Just pass current <see cref="UpdateContext"/> (not <see cref="ActionContext"/>, it will be created again for new action) to another action.
        /// <para></para>
        /// Will not be executed if <see cref="UpdateContext.ForceExit"/> was invoked.
        /// </summary>
        /// <param name="name">Name property from BotRouteAttribute.</param>
        public void StartAnotherAction(string name)
        {
            _mvcFeatures.StartAnotherAction(_ctx, name);
        }
    }
}
