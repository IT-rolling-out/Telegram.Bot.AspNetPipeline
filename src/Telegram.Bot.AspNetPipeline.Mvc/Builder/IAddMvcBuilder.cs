﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.ModelBinding;

namespace Telegram.Bot.AspNetPipeline.Mvc.Builder
{
    /// <summary>
    /// Used in AddMvc. Will be added to DI container.
    /// </summary>
    public interface IAddMvcBuilder
    {
        /// <summary>
        /// All will be registered as services.
        /// </summary>
        IList<Type> Controllers { get; set; }

        IServiceCollection ServiceCollection { get; }

        MvcOptions MvcOptions { get; }
    }


}
