using System.Threading.Tasks;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.ModelBinding.Binders
{
    public interface IModelBinder
    {
        Task Bind(ModelBindingContext modelBinderContext);
    }
}
