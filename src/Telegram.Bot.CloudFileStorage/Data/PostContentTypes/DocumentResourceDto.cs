using Telegram.Bot.CloudFileStorage.Consts;

namespace Telegram.Bot.CloudFileStorage.Data.PostContentTypes
{
    public class DocumentResourceDto : ResourceDto
    {
        public override PostResExtType ExtType
        {
            get => PostResExtType.Document;
            set { }
        }
    }
}