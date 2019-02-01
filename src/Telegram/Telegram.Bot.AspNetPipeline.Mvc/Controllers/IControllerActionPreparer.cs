using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers
{
    public interface IControllerActionPreparer
    {
        Task<UpdateProcessingDelegate> PrepareController(ControllerActionInfo controllerActionDescriptor);
    }
}
