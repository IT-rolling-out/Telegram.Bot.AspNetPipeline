using System.Collections.Generic;
using System.Threading.Tasks;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing.Routers
{
    public abstract class BaseStringMatchRouter:IRouter
    {
        protected BaseStringMatchRouter()
        {
            //TODO: Here actions dictionary must be invoked.
        }

        /// <summary>
        /// Note, that StringMatchRouter.SetTemplateMatchingStrings must not set any handler in RoutingContext.
        /// It can only customize routing by TemplateMatchingStrings. It can add|remove|edit
        /// current list of strings.
        /// <para></para>
        /// TemplateMatchingStrings used only for fast searching of [BotRoute(temlate)] in Dictionary.
        /// <para></para>
        /// After each IRouter processing mvc middleware will search all TemplateMatchingStrings in dictionary.
        /// If not find - call next router.
        /// </summary>
        protected IList<string> TemplateMatchingStrings { get; set; } = new List<string>();

        
        public async Task RouteAsync(RoutingContext routeContext)
        {
            await SetTemplateMatchingStrings(routeContext);

            //TODO: Here must be searching in dictionary with actions.
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Can only set TemplateMatchingStrings that used in searching of handler.
        /// </summary>
        protected abstract Task SetTemplateMatchingStrings(RoutingContext routeContext);
    }
}
