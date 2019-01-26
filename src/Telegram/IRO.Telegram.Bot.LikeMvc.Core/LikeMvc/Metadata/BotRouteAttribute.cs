using System;
using Telegram.Bot.Types.Enums;

namespace IRO.Telegram.Bot.ProcessingPipeline.LikeMvc.Metadata
{
    [AttributeUsage(AttributeTargets.Method)]
    public class BotRouteAttribute : Attribute
    {
        public BotRouteAttribute(string template, params UpdateType[] updateTypes)
        {
            Template = template;
            UpdateTypes = updateTypes;
        }

        public BotRouteAttribute(params UpdateType[] updateTypes) : this(null, updateTypes)
        {
        }

        /// <summary>
        /// The route template. May be null if the action has no attribute routes.
        /// </summary>
        public string Template { get; }

        public UpdateType[] UpdateTypes { get; }

        /// <summary>
        /// Gets the order of the route associated with a given action. This property determines
        /// the order in which routes get executed. Routes with a lower order value are tried first. In case a route
        /// doesn't specify a value, it gets a default order of 0.
        /// </summary>
        public int Order { get; set; }

        public string Name { get; set; }

        public RouteData GetRouteData()
        {
            var res = new RouteData(
                Template,
                Order,
                Name,
                UpdateTypes
            );
            return res;
        }
    }
}
