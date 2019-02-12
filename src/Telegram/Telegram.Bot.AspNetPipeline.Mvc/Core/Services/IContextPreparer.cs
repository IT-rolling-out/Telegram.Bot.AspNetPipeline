using Telegram.Bot.AspNetPipeline.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Core
{
    public interface IContextPreparer
    {
        /// <summary>
        /// Can return ControllerActionContext for controllers.
        /// </summary>
        ActionContext CreateContext(UpdateContext ctx, ActionDescriptor actionDescriptor);
    }
}
