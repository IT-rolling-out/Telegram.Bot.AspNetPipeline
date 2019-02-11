using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.MiddlewareServices
{
    public interface IControllerActionPreparer
    {
        Task<UpdateProcessingDelegate> PrepareController(ControllerActionDescriptor controllerActionDescriptor);
    }
}
