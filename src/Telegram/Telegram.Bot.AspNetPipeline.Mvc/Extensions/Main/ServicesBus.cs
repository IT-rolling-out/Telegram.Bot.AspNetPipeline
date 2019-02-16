using Telegram.Bot.AspNetPipeline.Mvc.Builder;
using Telegram.Bot.AspNetPipeline.Mvc.Routing;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Routers;

namespace Telegram.Bot.AspNetPipeline.Mvc.Extensions.Main
{
    /// <summary>
    /// Singleton registered in IOC container, but initialized after container created.
    /// <para></para>
    /// Used to inject services, that created after service container builded.
    /// </summary>
    public class ServicesBus : IMainRouterProvider, IOuterMiddlewaresInformerProvider,IMvcFeaturesProvider
    {
        /// <summary>
        /// Invoked in <see cref="MvcMiddleware"/>
        /// </summary>
        public void Init(
            MainRouter mainRouter,
            IOuterMiddlewaresInformer outerMiddlewaresInformer,
            IMvcFeatures mvcFeatures
            )
        {
            MainRouter = mainRouter;
            OuterMiddlewaresInformer = outerMiddlewaresInformer;
            MvcFeatures = mvcFeatures;
        }

        public MainRouter MainRouter { get; private set; }

        public IOuterMiddlewaresInformer OuterMiddlewaresInformer { get; private set; }

        public IMvcFeatures MvcFeatures { get; private set; }
    }
}
