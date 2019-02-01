using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers
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
