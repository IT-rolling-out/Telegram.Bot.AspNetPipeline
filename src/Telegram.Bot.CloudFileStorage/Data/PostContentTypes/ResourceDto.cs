using Telegram.Bot.CloudFileStorage.Consts;

namespace Telegram.Bot.CloudFileStorage.Data.PostContentTypes
{
    public class ResourceDto
    {
        public virtual PostResSourceType SourceType { get; set; }

        public string Url { get; set; }

        #region For TelegramPost SourceType.
        public string TgFileId { get; set; }

        public long TgBotId { get; set; }
        #endregion

        public virtual PostResExtType ExtType { get; set; }

        public string Name { get; set; }
    }
}
