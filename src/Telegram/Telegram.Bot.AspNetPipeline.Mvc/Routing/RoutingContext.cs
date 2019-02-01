using System.Collections.Generic;
using Telegram.Bot.AspNetPipeline.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing
{
    /// <summary>
    /// Used to describe and handle UpdateContext routing.
    /// <para></para>
    /// Note, that IRouter can set any handler in RoutingContext, like it can do in asp.net.
    /// IRouter can only customize routing by TemplateMatchingStrings. It can add|remove|edit
    /// current list of strings.
    /// <para></para>
    /// TemplateMatchingStrings used one by one only for fast searching of [BotRoute(temlate)] in indexed Dictionary.
    /// </summary>
    public class RoutingContext
    {
        public RoutingContext(UpdateContext updateContext)
        {
            UpdateContext = updateContext;
        }

        public UpdateContext UpdateContext { get; }

        /// <summary>
        /// Note, that IRouter can set any handler in RoutingContext, like it can do in asp.net.
        /// IRouter can only customize routing by TemplateMatchingStrings. It can add|remove|edit
        /// current list of strings.
        /// <para></para>
        /// TemplateMatchingStrings used one by one only for fast searching of [BotRoute(temlate)] in indexed Dictionary.
        /// </summary>
        public IList<string> TemplateMatchingStrings { get; } = new List<string>();

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
