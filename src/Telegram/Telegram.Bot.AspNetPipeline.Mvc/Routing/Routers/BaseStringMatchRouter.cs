using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing.Routers
{
    public abstract class BaseStringMatchRouter : IRouter
    {
        protected ILogger Logger { get; private set; }

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
        public async Task RouteAsync(RoutingContext routeContext)
        {
            if (Logger == null)
            {
                var loggerFactory = routeContext.UpdateContext.Services.GetRequiredService<ILoggerFactory>();
                Logger = loggerFactory.CreateLogger(GetType());
            }

            var templateMatchingStrings = new List<string>();
            await SetTemplateMatchingStrings(routeContext, templateMatchingStrings);
            
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                string allTemplates = "";
                foreach (var str in templateMatchingStrings)
                {
                    allTemplates += $"\"{str}\", ";
                }
                allTemplates = allTemplates.Trim();
                if (allTemplates.Length>0)
                    allTemplates=allTemplates.Remove(allTemplates.Length-1);
                Logger.LogTrace("Templates resolved from context:\n " + allTemplates);
            }

            if (templateMatchingStrings.Count == 0)
            {
                return;
            }

            var globalSearchBagProvider =
                routeContext.UpdateContext.Services.GetRequiredService<IGlobalSearchBagProvider>();
            var globalSearchBag = globalSearchBagProvider.Resolve();
            foreach (var fullMatchTemplate in templateMatchingStrings)
            {
                foreach (var orderScope in globalSearchBag.AllSorted())
                {
                    var templateScope = orderScope.FindTemplateScope(fullMatchTemplate);
                    if (templateScope != null)
                    {
                        var updType = routeContext.UpdateContext.Update.Type;
                        var actDesc = templateScope.FindOneByUpdateType(updType);
                        if (actDesc != null)
                        {
                            routeContext.ActionDescriptor = actDesc;
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Can only set TemplateMatchingStrings that used in searching of handler.
        /// </summary>
        protected abstract Task SetTemplateMatchingStrings(RoutingContext routeContext, IList<string> templateMatchingStrings);
    }
}
