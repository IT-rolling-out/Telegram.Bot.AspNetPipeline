using System.Collections.Generic;
using IRO.Common.Collections;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing
{
    /// <summary>
    /// Used to describe route.
    /// <para></para>
    /// Something like AttributeRouteInfo from ASP.NET.
    /// Can be used with controller method or any other route (ActionDescriptor) in mvc.
    /// </summary>
    public class RouteInfo
    {
        public RouteInfo(
            string template=null, 
            int order=0, 
            string name=null, 
            IEnumerable<UpdateType> updateTypes=null
            )
        {
            Template = template;
            Order = order;
            Name = name;
            if (updateTypes != null)
            {
                UpdateTypes = EnumerableExtensions.ToHashSet(updateTypes);
            }
        }

        /// <summary>
        /// The route template. May be null if the action has no attribute routes.
        /// <para></para>
        /// Can be null.
        /// </summary>
        public string Template { get; }

        /// <summary>
        /// Gets the order of the route associated with a given action. This property determines
        /// the order in which routes get executed. Routes with a lower order value are tried first. In case a route
        /// doesn't specify a value, it gets a default order of 0.
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// Case sensitive.
        /// <para></para>
        /// Can be null. Than will not be registered.
        /// </summary>
        public string Name { get;  }

        /// <summary>
        /// Used hash set for fast search.
        /// <para></para>
        /// Can be null. Null mean all update types.
        /// </summary>
        public HashSet<UpdateType> UpdateTypes { get; }

        public override string ToString()
        {
            return nameof(RouteInfo) + $"(Name=\"{Name}\", Template=\"{Template}\", Order={Order})";
        }
    }
}
