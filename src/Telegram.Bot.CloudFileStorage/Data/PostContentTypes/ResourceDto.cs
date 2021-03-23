using Telegram.Bot.CloudFileStorage.Consts;

namespace Telegram.Bot.CloudFileStorage.Data.PostContentTypes
{
    public class ResourceDto
    {
        public string TgFileId { get; set; }

        public long TgBotId { get; set; }

        public virtual PostResExtType ExtType { get; set; }

        public string Name { get; set; }
    }
}
