using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers;
using Telegram.Bot.AspNetPipeline.Mvc.Core.Services;
using Telegram.Bot.AspNetPipeline.Mvc.Extensions;
using Telegram.Bot.AspNetPipeline.Mvc.Extensions.Main;
using Telegram.Bot.AspNetPipeline.Mvc.Routing;

namespace Telegram.Bot.AspNetPipeline.Mvc.Builder
{
    public static class PipelineBuilderMvcExtensions
    {
        public static void UseMvc(
            this IPipelineBuilder @this,
            Action<IUseMvcBuilder> configureUseMvcBuilder = null)
        {
            var addMvcBuilder = @this.ServiceProvider.GetService<IAddMvcBuilder>();
            var useMvcBuilder = new UseMvcBuilder(@this.ServiceProvider);
            configureUseMvcBuilder?.Invoke(useMvcBuilder);
            var md = new MvcMiddleware(addMvcBuilder, useMvcBuilder);
            @this.UseMiddlware(md);
        }

        /// <summary>
        /// </summary>
        /// <param name="mvcOptions">AddMvcOptions.Default if null.</param>
        /// <returns></returns>
        public static void AddMvc(
            this ServiceCollectionWrapper serviceCollectionWrapper,
            MvcOptions mvcOptions = null,
            Action<IAddMvcBuilder> configureAddMvcBuilder = null)
        {
            var serv = serviceCollectionWrapper.Services;
            //First step. Register all services, that can be registered before IAddMvcBuilder configurations.
            AddServises_NotRequiredBuilder(serviceCollectionWrapper);

            //Second step. Init AddMvcBuilder parameters with MvcOptions to pass it to callback.
            mvcOptions = mvcOptions ?? new MvcOptions();
            var addMvcBuilder = InitAddMvcBuilder(serviceCollectionWrapper, mvcOptions);

            //Custom configs.
            configureAddMvcBuilder?.Invoke(addMvcBuilder);

            //Third step. Register all services based on IAddMvcBuilder.
            AddServises_RequiredBuilder(
                serviceCollectionWrapper,
                addMvcBuilder
                );
            
            //!Add controllers services.
            ControllersMiddlewareExtensions.AddControllersServices(serviceCollectionWrapper);

            //Finish. Register builders to be resolved in middleware.
            serv.AddSingleton<IAddMvcBuilder>(addMvcBuilder);
        }

        static void AddServises_NotRequiredBuilder(ServiceCollectionWrapper serviceCollectionWrapper)
        {
            var serv = serviceCollectionWrapper.Services;

            //Register services bus for services, created in MvcMiddleware.
            var servicesBus = new ServicesBus();
            serv.AddSingleton<ServicesBus>(servicesBus);
            serv.AddSingleton<IMainRouterProvider>(servicesBus);
            serv.AddSingleton<IOuterMiddlewaresInformerProvider>(servicesBus);
            serv.AddSingleton<IMvcFeaturesProvider>(servicesBus);
        }

        static void AddServises_RequiredBuilder(ServiceCollectionWrapper serviceCollectionWrapper, IAddMvcBuilder addMvcBuilder)
        {
            var serv = serviceCollectionWrapper.Services;
            foreach (var controllerType in addMvcBuilder.Controllers)
            {
                serv.AddTransient(controllerType);
            }

        }

        static IAddMvcBuilder InitAddMvcBuilder(ServiceCollectionWrapper serviceCollectionWrapper, MvcOptions mvcOptions)
        {
            var serv = serviceCollectionWrapper.Services;
            IAddMvcBuilder addMvcBuilder = new AddMvcBuilder(
                mvcOptions,
                serv
                );
            ControllersMiddlewareExtensions.InitAddMvcBuilder(mvcOptions, addMvcBuilder);
            return addMvcBuilder;
        }
    }
}
