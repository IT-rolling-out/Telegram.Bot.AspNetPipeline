using System.Collections.Generic;
using System.Text;
using Telegram.Bot.AspNetPipeline.Extensions.Serialization;

namespace Telegram.Bot.AspNetPipeline.Extensions.Logging
{
    public struct LoggingAdvancedOptions
    {
        public ILazySerializerFactory LazySerializerFactory { get; set; }
    }

}
