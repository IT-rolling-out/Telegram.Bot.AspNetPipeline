using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.ModelBinding.Binders
{
    /// <summary>
    /// Used only to aggregate all binders.
    /// </summary>
    public class MainModelBinder:IModelBinder
    {
        readonly IEnumerable<IModelBinder> _biders;

        public MainModelBinder(IEnumerable<IModelBinder> biders)
        {
            if (biders == null)
                throw new ArgumentNullException(nameof(biders));
            _biders = biders.ToList();
        }
    
        public async Task Bind(ModelBindingContext modelBinderContext)
        {
            foreach (var binder in _biders)
            {
                if (modelBinderContext.IsAllBinded())
                    return;
                await binder.Bind(modelBinderContext);
            }
        }
    }
}
