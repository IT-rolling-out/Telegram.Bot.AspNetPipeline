using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Core
{
    public class BotClientContext
    {
        public BotClientContext(ITelegramBotClient bot, User botInfo)
        {
            Bot = bot;
            BotInfo = botInfo;
        }

        public ITelegramBotClient Bot { get; }

        /// <summary>
        /// Updated only on <see cref="BotHandler.Setup"/>.
        /// </summary>
        public User BotInfo { get; }

        public string Username => BotInfo.Username;
    }
}
