namespace Telegram.Bot.AspNetPipeline.Core
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
