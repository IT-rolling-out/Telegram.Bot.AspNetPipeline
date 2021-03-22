using Telegram.Bot.CloudFileStorage.Consts;

namespace Telegram.Bot.CloudFileStorage.Data.PostContentTypes
{
    public class VideoNoteResourceDto : ResourceDto
    {
        public override PostResExtType ExtType
        {
            get => PostResExtType.VideoNote;
            set { }
        }
    }
}