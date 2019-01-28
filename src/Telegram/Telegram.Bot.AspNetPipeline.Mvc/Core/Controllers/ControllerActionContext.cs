using Telegram.Bot.AspNetPipeline.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Core.Controllers
{
    public class ControllerActionContext : ActionContext
    {
        public ControllerActionContext(
            UpdateContext updateContext,
            ControllerActionInfo controllerActionInfo,
            IMvcFeatures features
            ) 
            : base(updateContext, controllerActionInfo, features)
        {
        }

        /// <summary>
        /// Info about controller method. Casted ActionInfo.
        /// </summary>
        public ControllerActionInfo ControllerActionInfo => (ControllerActionInfo)ActionInfo;
    }


}
