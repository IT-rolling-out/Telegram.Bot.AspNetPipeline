using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.ModelBinding;

namespace Telegram.Bot.AspNetPipeline.Mvc.Builder
{
    /// <summary>
    /// Used in AddMvc.
    /// </summary>
    public interface IAddMvcBuilder
    {
        /// <summary>
        /// All will be registered as services.
        /// </summary>
        IList<Type> Controllers { get; }

        /// <summary>
        /// Without ModelBinderProvider and more simpler.
        /// But you can build your own asp-like model binding middleware, if needed.
        /// </summary>
        IList<IModelBinder> ModelBinders { get; }

        IServiceCollection ServiceCollection { get; }

    }


}
