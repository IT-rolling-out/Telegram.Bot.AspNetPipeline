using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers;

namespace Telegram.Bot.AspNetPipeline.Mvc.Builder
{
    public static class PipelineBuilderMvcExtensions
    {
        public static void UseMvc(this IPipelineBuilder @this, Action<IUseMvcBuilder> configureUseMvcBuilder = null)
        {
            var addMvcBuilder = @this.ServiceProvider.GetService<IAddMvcBuilder>();
            var useMvcBuilder = @this.ServiceProvider.GetService<IUseMvcBuilder>();
            configureUseMvcBuilder?.Invoke(useMvcBuilder);
            var md = new MvcMiddleware(addMvcBuilder, useMvcBuilder);
            @this.UseMiddlware(md);
        }

        /// <summary>
        /// </summary>
        /// <param name="addMvcOptions">AddMvcOptions.Default if null.</param>
        /// <returns></returns>
        public static IAddMvcBuilder AddMvc(this IServiceCollection @this, AddMvcOptions addMvcOptions = null)
        {
            //IAddMvcBuilder and IUseMvcBuilder can be overrided with ioc.
            addMvcOptions = addMvcOptions ?? new AddMvcOptions();
            IList<Type> controllers = null;
            if (addMvcOptions.FindControllersByReflection)
            {
                controllers = ControllersTypesSearch.FindAllControllers();
            }
            controllers = controllers ?? new List<Type>();

            var addMvcBuilder =new  AddMvcBuilder(
                addMvcOptions, 
                controllers, 
                @this
                );
            @this.AddSingleton<IAddMvcBuilder>(addMvcBuilder);
            @this.AddSingleton<IUseMvcBuilder, UseMvcBuilder>();
            return addMvcBuilder;
        }
    }
}
