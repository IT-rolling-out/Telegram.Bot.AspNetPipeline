using Telegram.Bot.CloudFileStorage.Consts;

namespace Telegram.Bot.CloudFileStorage.Data.PostContentTypes
{
    public class ImageResourceDto : ResourceDto
    {
        public override PostResExtType ExtType
        {
            get => PostResExtType.Image;
            set { }
        }
    }
}