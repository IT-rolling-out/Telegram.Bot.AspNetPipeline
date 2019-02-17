using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing.Routers
{
    public class FullMatchRouter : BaseStringMatchRouter
    {
        protected override async Task SetTemplateMatchingStrings(RoutingContext routeContext, IList<string> templateMatchingStrings)
        {
            var template = routeContext.UpdateContext.Message?.Text?.Trim();
            if (template != null)
                templateMatchingStrings.Add(template);

            //With empty string template. Empty template priority is lower than priority of other templates.
            templateMatchingStrings.Add("");
        }
    }
}
