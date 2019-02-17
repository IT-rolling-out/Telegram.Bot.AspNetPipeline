using Telegram.Bot.AspNetPipeline.Mvc.Builder;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.ModelBinding;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.ModelBinding.Binders;
using Telegram.Bot.AspNetPipeline.Mvc.Extensions.MvcFeatures;
using Telegram.Bot.AspNetPipeline.Mvc.Routing;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Routers;

namespace Telegram.Bot.AspNetPipeline.Mvc.Extensions.Main
{
    /// <summary>
    /// Singleton registered in IOC container, but initialized after container created.
    /// <para></para>
    /// Used to inject services, that created after service container builded.
    /// </summary>
    public class ServicesBus : 
        IMainRouterProvider, 
        IOuterMiddlewaresInformerProvider,
        IMvcFeaturesProvider,
        IMainModelBinderProvider
    {
        /// <summary>
        /// Invoked in <see cref="MvcMiddleware"/>
        /// </summary>
        public void Init(
            MainRouter mainRouter,
            IOuterMiddlewaresInformer outerMiddlewaresInformer,
            IMvcFeatures mvcFeatures,
            MainModelBinder mainModelBinder
            )
        {
            MainRouter = mainRouter;
            OuterMiddlewaresInformer = outerMiddlewaresInformer;
            MvcFeatures = mvcFeatures;
            MainModelBinder = mainModelBinder;
        }

        public MainRouter MainRouter { get; private set; }

        public IOuterMiddlewaresInformer OuterMiddlewaresInformer { get; private set; }

        public IMvcFeatures MvcFeatures { get; private set; }

        public MainModelBinder MainModelBinder { get; private set; }
    }
}
