using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Core.Services;

namespace Telegram.Bot.AspNetPipeline.Mvc.Core
{
    /// <summary>
    /// Support controllers context too.
    /// </summary>
    public class ContextPreparer:IContextPreparer
    {
        /// <summary>
        /// Can return ControllerActionContext for controllers.
        /// </summary>
        public ActionContext CreateContext(UpdateContext ctx, ActionDescriptor actionDescriptor)
        {
            if (actionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                //Controllers context.
                return new ControllerActionContext(ctx, controllerActionDescriptor);
            }
            //Startup delegates context.
            return new ActionContext(ctx, actionDescriptor);
        }
    }
}
