using System;
using System.Collections.Generic;
using System.Reflection;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.ModelBinding
{
    public class ModelBinderContext
    {
        public ControllerActionInfo ControllerActionInfo { get; }

        public ParameterInfo[] Parameters { get; }

        public ModelBinderContext(ControllerActionInfo controllerActionInfo, ParameterInfo[] parameters)
        {
            ControllerActionInfo = controllerActionInfo;
            Parameters = parameters;
        }

        /// <summary>
        /// Set value. If was binded - ignore it.
        /// If type cant be casted - throw exception.
        /// </summary>
        public void BindValue(string parameterName, object value)
        {
            throw new NotImplementedException();
        }

        public bool IsBinded(string parameterName)
        {
            throw new NotImplementedException();
        }

        public bool IsAllBinded()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ParameterInfo> GetNotBinded()
        {
            throw new NotImplementedException();
        }

        public object[] ToMethodParameters()
        {
            throw new NotImplementedException();
        }
    }
}
