using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Telegram.Bot.CloudFileStorage.Consts
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PostResExtType
    {
        Unknown,
        Image,
        Video,
        Audio,
        Animation,
        Voice,
        VideoNote,
        Document,
        Forward,
        Sticker
    }
}