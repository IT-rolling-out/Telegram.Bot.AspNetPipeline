using Telegram.Bot.AspNetPipeline.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Core
{
    /// <summary>
    /// Something like ControllerContext, but more abstract. 
    /// </summary>
    public class ActionContext
    {
        public ActionContext(UpdateContext updateContext, ActionInfo actionInfo, IMvcFeatures features)
        {
            UpdateContext = updateContext;
            ActionInfo = actionInfo;
            Features = features;
        }

        public UpdateContext UpdateContext { get; }

        public ActionInfo ActionInfo { get; }

        public IMvcFeatures Features { get; }
    }


}
