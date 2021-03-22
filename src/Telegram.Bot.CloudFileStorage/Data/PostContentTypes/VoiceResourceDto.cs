using Telegram.Bot.CloudFileStorage.Consts;

namespace Telegram.Bot.CloudFileStorage.Data.PostContentTypes
{
    public class VoiceResourceDto : ResourceDto
    {
        public override PostResExtType ExtType
        {
            get => PostResExtType.Voice;
            set { }
        }
    }
}