using Telegram.Bot.CloudFileStorage.Consts;

namespace Telegram.Bot.CloudFileStorage.Data.PostContentTypes
{
    public class AudioResourceDto : ResourceDto
    {
        public override PostResExtType ExtType
        {
            get => PostResExtType.Audio;
            set { }
        }
    }
}