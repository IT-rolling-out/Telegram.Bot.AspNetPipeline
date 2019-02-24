using System;
using Newtonsoft.Json;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing;

namespace Telegram.Bot.AspNetPipeline.Mvc.Builder
{
    public class MvcOptions:ICloneable
    {
        /// <summary>
        /// Default is true. If false - controllers list in <see cref="IAddMvcBuilder"/> will be empty.
        /// </summary>
        public bool FindControllersByReflection { get; set; } = true;

        /// <summary>
        /// If tue - mvc middleware will set validator to BotExt ReadMessageAsync.
        /// Allow to use controller methods with hight priority before BotExt.
        /// <para></para>
        /// Default is true. If you disable it - ReadMessageAsync callbacks will be always handled first,
        /// even if you use commands in you message.
        /// <para></para>
        /// Highly recommended not to disable it.
        /// </summary>
        public bool ConfigureBotExtWithMvc { get; set; } = true;

        /// <summary>
        /// Same to controllers BotRouteAttribute order.
        /// All route actions, that has bigger or equals order than BotExtOrder value (actions with lower priority)
        /// will be processed after ReadMessageAsync callbacks.
        /// <para></para>
        /// Default value is 0.
        /// </summary>
        public int BotExtOrder { get; set; } = 0;

        /// <summary>
        /// Limit of StartAnotherAction calls for one <see cref="UpdateContext"/>.
        /// Default is 5.
        /// </summary>
        public int StartAnotherActionMaxStackLevel { get; set; } = 5;

        /// <summary>
        /// Throw exceptions if found completly equals <see cref="RouteInfo"/>s.
        /// Recommended only in debug, because it's make app startup slower.
        /// <para></para>
        /// Default is false.
        /// </summary>
        public bool CheckEqualsRouteInfo { get; set; }

        public object Clone()
        {
            //Lazy deep copy.
            var str= JsonConvert.SerializeObject(this);
            var clone = JsonConvert.DeserializeObject(str, GetType());
            return clone;
        }
    }
}
