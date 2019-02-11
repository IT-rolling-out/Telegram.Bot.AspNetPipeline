using System;
using System.Collections.Generic;
using System.Reflection;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.ModelBinding
{
    public class ModelBinderContext
    {
        public ControllerActionDescriptor ControllerActionDescriptor { get; }

        public ParameterInfo[] Parameters { get; }

        public ModelBinderContext(ControllerActionDescriptor controllerActionDescriptor, ParameterInfo[] parameters)
        {
            ControllerActionDescriptor = controllerActionDescriptor;
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
