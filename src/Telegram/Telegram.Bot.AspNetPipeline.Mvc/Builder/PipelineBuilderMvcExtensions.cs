using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers;

namespace Telegram.Bot.AspNetPipeline.Mvc.Builder
{
    public static class PipelineBuilderMvcExtensions
    {
        public static void UseMvc(
            this IPipelineBuilder @this,
            Action<IUseMvcBuilder> configureUseMvcBuilder = null)
        {
            var addMvcBuilder = @this.ServiceProvider.GetService<IAddMvcBuilder>();
            var useMvcBuilder = @this.ServiceProvider.GetService<IUseMvcBuilder>();
            var md = new MvcMiddleware(addMvcBuilder, useMvcBuilder);
            @this.UseMiddlware(md);
        }

        /// <summary>
        /// </summary>
        /// <param name="addMvcOptions">AddMvcOptions.Default if null.</param>
        /// <returns></returns>
        public static void AddMvc(
            this ServiceCollectionWrapper @this,
            MvcOptions addMvcOptions = null,
            Action<IAddMvcBuilder> configureAddMvcBuilder = null)
        {
            MvcMiddleware.RegisterServices(@this, addMvcOptions, configureAddMvcBuilder);
        }
    }
}
