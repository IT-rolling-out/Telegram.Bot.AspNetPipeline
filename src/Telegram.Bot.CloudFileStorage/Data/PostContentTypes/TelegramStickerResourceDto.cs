using Telegram.Bot.CloudFileStorage.Consts;

namespace Telegram.Bot.CloudFileStorage.Data.PostContentTypes
{
    /// <summary>
    /// Can be used only with telegram.
    /// </summary>
    public class TelegramStickerResourceDto : ResourceDto
    {

        public override PostResSourceType SourceType
        {
            get => PostResSourceType.TelegramPost;
            set { }
        }

        public override PostResExtType ExtType
        {
            get => PostResExtType.Sticker;
            set { }
        }

    }
}