using System;
using IRO.Common.Serialization;

namespace Telegram.Bot.AspNetPipeline.Extensions.Logging
{
    [Obsolete("In old versions you can allow logging system to serialize loggeing context to json. " +
              "Now current function is deprecated, all data that you need showed with ToString().")]
    public struct LoggingAdvancedOptions
    {
        public ILazySerializerFactory LazySerializerFactory { get; set; }
    }

}
