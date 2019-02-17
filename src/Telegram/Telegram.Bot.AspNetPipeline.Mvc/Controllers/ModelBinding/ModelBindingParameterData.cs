using System.Reflection;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.ModelBinding
{
    class ModelBindingParameterData
    {
        public bool IsBinded { get; set; }

        public object Value { get; set; }

        public ParameterInfo Info { get; set; }
    }
}
