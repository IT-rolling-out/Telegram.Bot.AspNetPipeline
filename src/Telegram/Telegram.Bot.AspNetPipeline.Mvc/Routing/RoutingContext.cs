using System.Collections.Generic;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing
{
    /// <summary>
    /// Used to describe and handle UpdateContext routing.
    /// </summary>
    public class RoutingContext
    {
        public RoutingContext(UpdateContext updateContext)
        {
            UpdateContext = updateContext;
        }

        public UpdateContext UpdateContext { get; }

        /// <summary>
        /// Set it if route match.
        /// </summary>
        public RouteDescriptionData RouteDescriptionData { get; set; } = RouteDescriptionData.Empty;

        #region Properties bag.
        IDictionary<object, object> _properties;

        /// <summary>
        /// Stores arbitrary metadata properties associated with current type.
        /// </summary>
        public IDictionary<object, object> Properties
        {
            get
            {
                if (_properties == null)
                {
                    _properties = new Dictionary<object, object>();
                }
                return _properties;
            }
        }
        #endregion
    }
}
