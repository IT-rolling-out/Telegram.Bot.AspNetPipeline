using Telegram.Bot.CloudFileStorage.Consts;

namespace Telegram.Bot.CloudFileStorage.Data.PostContentTypes
{
    public class AnimationResourceDto : ResourceDto
    {
        public override PostResExtType ExtType
        {
            get => PostResExtType.Animation;
            set { }
        }
    }
}