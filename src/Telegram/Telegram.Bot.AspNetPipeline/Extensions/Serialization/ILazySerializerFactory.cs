namespace Telegram.Bot.AspNetPipeline.Extensions.Serialization
{
    public interface ILazySerializerFactory
    {
        LazySerializer<T> Create<T>(T value);
    }
}
