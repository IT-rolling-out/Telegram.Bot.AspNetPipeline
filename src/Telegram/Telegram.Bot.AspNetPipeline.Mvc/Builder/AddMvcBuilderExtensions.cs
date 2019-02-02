using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers;

namespace Telegram.Bot.AspNetPipeline.Mvc.Builder
{
    public static class AddMvcBuilderExtensions
    {
        /// <summary>
        /// Default factory use ioc to resolve controllers.
        /// </summary>
        public static IAddMvcBuilder AddControllersFactory<TControllersFactory>(this IAddMvcBuilder @this)
            where TControllersFactory : class, IControllersFactory
        {
            @this.ServiceCollection.AddSingleton<IControllersFactory, TControllersFactory>();
            return @this;
        }

        /// <summary>
        /// Default factory use ioc to resolve controllers.
        /// </summary>
        public static IAddMvcBuilder AddControllersFactory(this IAddMvcBuilder @this, IControllersFactory controllersFactory)
        {
            @this.ServiceCollection.AddSingleton<IControllersFactory>(controllersFactory);
            return @this;
        }
    }
}
