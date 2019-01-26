using Telegram.Bot.Types.Enums;

namespace IRO.Telegram.Bot.ProcessingPipeline.LikeMvc
{
    public class RouteData
    {
        public RouteData(string template, int order, string name, UpdateType[] updateTypes)
        {
            Template = template;
            Order = order;
            Name = name;
            UpdateTypes = updateTypes;
        }

        /// <summary>
        /// The route template. May be null if the action has no attribute routes.
        /// </summary>
        public string Template { get; }

        /// <summary>
        /// Gets the order of the route associated with a given action. This property determines
        /// the order in which routes get executed. Routes with a lower order value are tried first. In case a route
        /// doesn't specify a value, it gets a default order of 0.
        /// </summary>
        public int Order { get; }

        public string Name { get;  }

        public UpdateType[] UpdateTypes { get; }
    }
}
