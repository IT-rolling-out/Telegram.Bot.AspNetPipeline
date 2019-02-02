using System.Threading.Tasks;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.ModelBinding
{
    public interface IModelBinder
    {
        Task PrepareController(ModelBinderContext modelBinderContext);
    }
}
