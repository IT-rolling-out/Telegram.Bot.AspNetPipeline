using Telegram.Bot.CloudFileStorage.Consts;

namespace Telegram.Bot.CloudFileStorage.Data.PostContentTypes
{
    /// <summary>
    /// Can be used only with telegram.
    /// </summary>
    public class TelegramForwardResourceDto : ResourceDto
    {

        public override PostResSourceType SourceType
        {
            get => PostResSourceType.TelegramPost;
            set { }
        }

        public override PostResExtType ExtType
        {
            get => PostResExtType.Forward;
            set { }
        }

        /// <summary>
        /// Post message id.
        /// </summary>
        public int TgForwardMessageId { get; set; }

        /// <summary>
        /// Id of channel or something like this.
        /// </summary>
        public long TgForwardChatId { get; set; }

        /// <summary>
        /// Url of channel or something like this.
        /// </summary>
        public string ForwardUrl { get; set; }

    }
}
