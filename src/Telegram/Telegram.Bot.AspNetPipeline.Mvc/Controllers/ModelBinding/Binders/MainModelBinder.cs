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
        readonly IEnumerable<IModelBinder> _binders;

        public MainModelBinder(IEnumerable<IModelBinder> binders)
        {
            if (binders == null)
                throw new ArgumentNullException(nameof(binders));
            _binders = binders.ToList();
        }
    
        public async Task Bind(ModelBindingContext modelBinderContext)
        {
            foreach (var binder in _binders)
            {
                if (modelBinderContext.IsAllBinded())
                    return;
                await binder.Bind(modelBinderContext);
            }
        }
    }
}
