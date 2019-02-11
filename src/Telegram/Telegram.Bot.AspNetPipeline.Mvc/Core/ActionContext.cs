using Telegram.Bot.AspNetPipeline.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Core
{
    /// <summary>
    /// Something like ControllerContext, but more abstract. 
    /// </summary>
    public class ActionContext
    {
        public ActionContext(UpdateContext updateContext, ActionDescriptor actionDescriptor, IMvcFeatures features)
        {
            UpdateContext = updateContext;
            ActionDescriptor = actionDescriptor;
            Features = features;
        }

        public UpdateContext UpdateContext { get; }

        public ActionDescriptor ActionDescriptor { get; }

        public IMvcFeatures Features { get; }
    }


}
