using Telegram.Bot.CloudFileStorage.Consts;

namespace Telegram.Bot.CloudFileStorage.Data.PostContentTypes
{
    public class VideoResourceDto : ResourceDto
    {
        public override PostResExtType ExtType
        {
            get => PostResExtType.Video;
            set { }
        }
    }
}