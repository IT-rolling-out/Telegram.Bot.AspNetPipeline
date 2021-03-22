using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Telegram.Bot.CloudFileStorage.Consts
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PostResSourceType
    {
        Url,
        TelegramPost
    }
}