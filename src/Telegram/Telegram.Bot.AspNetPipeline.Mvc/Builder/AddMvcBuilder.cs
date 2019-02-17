using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.ModelBinding;

namespace Telegram.Bot.AspNetPipeline.Mvc.Builder
{
    internal class AddMvcBuilder : IAddMvcBuilder
    {
        /// <summary>
        /// All will be registered as services.
        /// </summary>
        public IList<Type> Controllers  { get; set; } = new List<Type>();

        public IServiceCollection ServiceCollection { get; }

        public MvcOptions MvcOptions { get; }

        public AddMvcBuilder(
            MvcOptions mvcOptions,
            IServiceCollection serviceCollection
            )
        {
            ServiceCollection = serviceCollection;
            MvcOptions = mvcOptions;
        }
    }


}
