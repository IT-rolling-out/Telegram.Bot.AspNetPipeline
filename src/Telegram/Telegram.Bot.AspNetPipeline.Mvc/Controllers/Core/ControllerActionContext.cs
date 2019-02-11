using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core
{
    /// <summary>
    /// Static data about registered method or method.
    /// </summary>
    public class ControllerActionContext : ActionContext
    {
        public ControllerActionContext(
            UpdateContext updateContext,
            ControllerActionDescriptor controllerActionDescriptor,
            IMvcFeatures features
            ) 
            : base(updateContext, controllerActionDescriptor, features)
        {
        }

        /// <summary>
        /// Info about controller method. Casted ActionInfo.
        /// </summary>
        public new ControllerActionDescriptor ActionDescriptor => (ControllerActionDescriptor)base.ActionDescriptor;
    }


}
