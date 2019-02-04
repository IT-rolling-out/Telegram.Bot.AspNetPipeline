using System.Collections.Generic;
using System.Text;
using Telegram.Bot.AspNetPipeline.Extensions.Serialization;

namespace Telegram.Bot.AspNetPipeline.Extensions.Logging
{
    public struct LoggingAdvancedOptions
    {
        /// <summary>
        /// If true - UpdateContext object will be serialized as json in default services, when write scoped logs.
        /// Warning! Json serialization logging on highload bot will significantly load the processor.
        /// <para></para>
        /// False by default.
        /// <para></para>
        /// To use current function - use UpdateScope extension GetLoggerScope().
        /// </summary>
        public bool LoggingWithSerialization { get; set; }

        public ILazySerializerFactory LazySerializerFactory { get; set; }
    }

}
