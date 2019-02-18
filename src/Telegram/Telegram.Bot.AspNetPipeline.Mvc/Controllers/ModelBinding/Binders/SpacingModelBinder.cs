using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.ModelBinding.Binders
{
    /// <summary>
    /// Default model binder. Resolve parameters by their position after command text.
    /// Split by space and use <see cref="JsonConvert"></see> to convert it (but stings used as is, without escape).
    /// <para></para>
    /// Convenient to use for integer parametes or words.
    /// </summary>
    public class SpacingModelBinder:IModelBinder
    {
        ILogger _logger;

        public async Task Bind(ModelBindingContext modelBinderContext)
        {
            if (_logger == null)
            {
                var loggerFactory = modelBinderContext.ControllerContext
                    .UpdateContext.Services.GetRequiredService<ILoggerFactory>();
                _logger = loggerFactory.CreateLogger(GetType());
            }

            try
            {
                var msgText = modelBinderContext.ControllerContext.UpdateContext.Update?.Message?.Text;
                if (msgText != null)
                {
                    msgText = msgText.Replace("\t", " ");
                    var arr = msgText.Split(' ');
                    var notWhiteSpaceList = new List<string>();
                    foreach (var str in arr)
                    {
                        if (str != "")
                            notWhiteSpaceList.Add(str);
                    }

                    //Remove cmd name.
                    if (notWhiteSpaceList[0].StartsWith("/"))
                    {
                        notWhiteSpaceList.RemoveAt(0);
                    }

                    for (int i = 0; i < notWhiteSpaceList.Count; i++)
                    {
                        var valueStr = notWhiteSpaceList[i];
                        var param = modelBinderContext.Parameters[i];

                        //Ignore binded.
                        if (modelBinderContext.IsBinded(param.Name))
                            continue;

                        //Convert.
                        object value = null;
                        if (param.ParameterType.IsAssignableFrom(typeof(string)))
                        {
                            value = valueStr;
                        }
                        else
                        {
                            try
                            {
                                value = JsonConvert.DeserializeObject(
                                    valueStr,
                                    param.ParameterType
                                );
                            }
                            catch
                            {
                            }
                        }

                        modelBinderContext.BindValue(param.Name, value);
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error while binding model.");
            }
        }
    }
}
