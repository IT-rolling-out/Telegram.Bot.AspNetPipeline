using System;

namespace Telegram.Bot.AspNetPipeline.Mvc.Builder
{
    public class MvcOptions
    {
        /// <summary>
        /// Default is true. Find controllers and register them in ioc as services.
        /// </summary>
        public bool FindControllersByReflection { get; set; } = true;

        /// <summary>
        /// Set validator to BotExt ReadMessageAsync.
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
        /// will be executed after ReadMessageAsync callback on conflicts.
        /// <para></para>
        /// Default value is null.
        /// </summary>
        public int BotExtOrder { get; set; } = 0;
    }
}
