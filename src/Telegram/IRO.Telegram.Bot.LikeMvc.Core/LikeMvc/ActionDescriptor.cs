using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace IRO.Telegram.Bot.ProcessingPipeline.LikeMvc
{
    public class ActionDescriptor
    {
        public ActionDescriptor(MethodInfo methodInfo, Type controllerType, RouteData routeData)
        {
            MethodInfo = methodInfo;
            ControllerType = controllerType;
            RouteData = routeData;
        }

        public MethodInfo MethodInfo { get;  }

        public Type ControllerType{ get; }
       
        public RouteData RouteData { get; }

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
                    _properties = new ConcurrentDictionary<object, object>();
                }
                return _properties;
            }
        }
        #endregion
    }
}
