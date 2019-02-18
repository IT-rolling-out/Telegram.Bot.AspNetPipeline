using IRO.Common.Serialization;

namespace Telegram.Bot.AspNetPipeline.Extensions.Logging
{
    public struct LoggingAdvancedOptions
    {
        public ILazySerializerFactory LazySerializerFactory { get; set; }
    }

}
