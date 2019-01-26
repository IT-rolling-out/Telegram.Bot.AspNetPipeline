using Telegram.Bot;

namespace IRO.Telegram.Bot.ProcessingPipeline.Core
{
    public class BotClientContext
    {
        public BotClientContext(ITelegramBotClient bot)
        {
            Bot = bot;
        }

        public ITelegramBotClient Bot { get; }
    }
}
