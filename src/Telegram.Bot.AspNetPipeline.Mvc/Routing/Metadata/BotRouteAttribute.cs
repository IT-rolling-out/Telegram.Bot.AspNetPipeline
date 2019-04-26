using System;
using Telegram.Bot.AspNetPipeline.Mvc.Extensions.MvcFeatures;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing.Metadata
{
    [AttributeUsage(AttributeTargets.Method)]
    public class BotRouteAttribute : Attribute
    {
        /// <summary>
        /// </summary>
        /// <param name="updateTypes">Null or empty mean all update types..</param>
        public BotRouteAttribute(string template, params UpdateType[] updateTypes)
        {
            Template = template;
            UpdateTypes = updateTypes;
        }

        /// <summary>
        /// Recommend to set order 1 or more for routes witout template.
        /// </summary>
        /// <param name="updateTypes">Null or empty mean all update types.</param>
        public BotRouteAttribute(params UpdateType[] updateTypes) : this(null, updateTypes)
        {
        }

        /// <summary>
        /// The route template. May be null if the action has no attribute routes.
        /// </summary>
        public string Template { get; }

        /// <summary>
        /// Null or empty mean all update types.
        /// </summary>
        public UpdateType[] UpdateTypes { get; }

        /// <summary>
        /// Gets the order of the route associated with a given action. This property determines
        /// the order in which routes get executed. Routes with a lower order value are tried first. In case a route
        /// doesn't specify a value, it gets a default order of 0.
        /// <para></para>
        /// NOTE: Priority of methods without template is always lower than proioryty of other methods.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Can be used to find action. Used in <see cref="IMvcFeatures.StartAnotherAction"/>.
        /// </summary>
        public string Name { get; set; }

        public RouteInfo GetRouteInfo()
        {
            var res = new RouteInfo(
                Template,
                Order,
                Name,
                UpdateTypes
            );
            return res;
        }
    }
}
