using System.Collections.Generic;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Core.Services;
using Telegram.Bot.AspNetPipeline.Mvc.Extensions;

namespace Telegram.Bot.AspNetPipeline.Mvc.Core
{
    /// <summary>
    /// Something like ControllerContext, but more abstract.
    /// <para></para>
    /// Created on each request.
    /// </summary>
    public class ActionContext
    {
        public ActionContext(UpdateContext updateContext, ActionDescriptor actionDescriptor)
        {
            UpdateContext = updateContext;
            ActionDescriptor = actionDescriptor;
        }

        public UpdateContext UpdateContext { get; }

        public ActionDescriptor ActionDescriptor { get; }

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
