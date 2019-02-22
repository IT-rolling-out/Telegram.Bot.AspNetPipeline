using System;
using System.Collections.Generic;
using System.Reflection;
using Telegram.Bot.AspNetPipeline.Exceptions;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.ModelBinding
{
    public class ModelBindingContext
    {
        IDictionary<string, ModelBindingParameterData> _dataByParamName = new Dictionary<string, ModelBindingParameterData>();

        public ControllerActionContext ControllerContext { get; }

        /// <summary>
        /// Proxy to ControllerContext.ActionDescriptor.Parameters.
        /// </summary>
        public ParameterInfo[] Parameters => ControllerContext.ActionDescriptor.Parameters;

        public ModelBindingContext(ControllerActionContext controllerContext)
        {
            ControllerContext = controllerContext
                ?? throw new ArgumentNullException(nameof(controllerContext));
            foreach (var param in Parameters)
            {
                _dataByParamName[param.Name] = new ModelBindingParameterData()
                {
                    Info = param
                };
            }
        }

        /// <summary>
        /// Set value. If was binded - override it.
        /// If type cant be casted - throw exception.
        /// </summary>
        public void BindValue(string parameterName, object value)
        {
            var data = _dataByParamName[parameterName];

            var t = data.Info.ParameterType;
            var castException = new TelegramAspException($"Model binding exception. Object '{value}' can't be used as '{t}'.");

            if ((value != null) && !t.IsAssignableFrom(value.GetType()))
                throw castException;
            if (value == null && t.IsValueType)
                throw castException;

            data.IsBinded = true;
            data.Value = value;

        }

        public bool IsBinded(string parameterName)
        {
            var data = _dataByParamName[parameterName];
            return data.IsBinded;
        }

        public bool IsAllBinded()
        {
            foreach (var item in _dataByParamName)
            {
                if (!item.Value.IsBinded)
                    return false;
            }
            return true;
        }

        public IEnumerable<ParameterInfo> GetNotBinded()
        {
            var res = new List<ParameterInfo>();
            foreach (var item in _dataByParamName)
            {
                if (item.Value.IsBinded)
                    continue;
                res.Add(item.Value.Info);
            }
            return res;
        }

        /// <summary>
        /// For not binded return default of parameter type.
        /// </summary>
        public object[] ToMethodParameters()
        {
            var res = new object[Parameters.Length];
            for (int i = 0; i < Parameters.Length; i++)
            {
                var param = Parameters[i];
                var data = _dataByParamName[param.Name];
                var value = data.Value;
                if (value == null && param.ParameterType.IsValueType)
                {
                    value = Activator.CreateInstance(param.ParameterType);
                }
                res[i] = value;
            }
            return res;
        }
    }
}
