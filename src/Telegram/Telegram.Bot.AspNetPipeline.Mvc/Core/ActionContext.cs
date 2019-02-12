using System.Collections.Generic;
using Telegram.Bot.AspNetPipeline.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Core
{
    /// <summary>
    /// Something like ControllerContext, but more abstract.
    /// <para></para>
    /// Created on each request.
    /// </summary>
    public class ActionContext
    {
        public ActionContext(UpdateContext updateContext, ActionDescriptor actionDescriptor, IMvcFeatures features)
        {
            UpdateContext = updateContext;
            ActionDescriptor = actionDescriptor;
            Features = features;
        }

        public UpdateContext UpdateContext { get; }

        public ActionDescriptor ActionDescriptor { get; }

        /// <summary>
        /// Allow you to interract with mvc middleware.
        /// </summary>
        public IMvcFeatures Features { get; }

        #region Properties bag.
        IDictionary<object, object> _properties;

        /// <summary>
        /// Stores arbitrary metadata properties associated with current object.
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
