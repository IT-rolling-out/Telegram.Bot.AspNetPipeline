using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.MiddlewareServices;

namespace Telegram.Bot.AspNetPipeline.Mvc.Builder
{
    public static class AddMvcBuilderExtensions
    {
        /// <summary>
        /// MVC service.
        /// <para></para>
        /// Default factory use ioc to resolve controllers.
        /// </summary>
        public static void AddControllersFactory<TControllersFactory>(this ServiceCollectionWrapper @this)
            where TControllersFactory : class, IControllersFactory
        {
            @this.Services.AddSingleton<IControllersFactory, TControllersFactory>();
        }

        /// <summary>
        /// MVC service.
        /// <para></para>
        /// Default factory use ioc to resolve controllers.
        /// </summary>
        public static void AddControllersFactory(this ServiceCollectionWrapper @this, IControllersFactory controllersFactory)
        {
            @this.Services.AddSingleton<IControllersFactory>(controllersFactory);
        }
    }
}
