using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing.Routers
{
    public class FullMatchRouter : BaseStringMatchRouter
    {
        protected override async Task SetTemplateMatchingStrings(RoutingContext routeContext, IList<string> templateMatchingStrings)
        {
            var template = routeContext.UpdateContext.Message?.Text?.Trim();
            if (template != null)
            {

                //Remove all text after first white space. Useful for cmd names.
                template = template.Replace("\t", " ");
                int firstEmptyIndex = template.IndexOf(" ");
                string beforeFirstSpaceTemplate = firstEmptyIndex < 0 ? template : template.Remove(firstEmptyIndex);

                if (routeContext.UpdateContext.IsChattingWithBot == true)
                {
                    //In bot chat we expect command without bot name.
                    templateMatchingStrings.Add(beforeFirstSpaceTemplate);
                }

                //Add commands from group. Example: "/cmd@bot_name".
                //But they can be used in chat with bot too.
                var botName = "@" + routeContext.UpdateContext.BotContext.Username;
                int atCharIndex = beforeFirstSpaceTemplate.IndexOf(botName);
                if (atCharIndex > 0)
                {
                    string cmdWithoutBotName = beforeFirstSpaceTemplate.Remove(atCharIndex, botName.Length);
                    templateMatchingStrings.Add(cmdWithoutBotName);
                }
            }

            //With empty string template. Empty template priority is lower than priority of other templates.
            templateMatchingStrings.Add("");
        }
    }
}
