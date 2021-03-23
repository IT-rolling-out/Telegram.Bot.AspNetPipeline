using System;
using System.Threading.Tasks;

namespace Telegram.Bot.CloudFileStorage.BotsProviders
{
    public class OneTelegramBotProvider : ITelegramBotsProvider
    {
        readonly ITelegramBotClient _telegramBotClient;

        public OneTelegramBotProvider(ITelegramBotClient telegramBotClient)
        {
            _telegramBotClient = telegramBotClient ?? throw new ArgumentNullException(nameof(telegramBotClient));
        }

        public async Task<ITelegramBotClient> GetMainBotClient()
        {
            return _telegramBotClient;
        }

        public async Task<ITelegramBotClient> GetBotClient(long id)
        {
            if (_telegramBotClient.BotId != id)
            {
                throw new Exception($"Current bot provider provide only one bot, bot with id '{id}' not found.");
            }
            return _telegramBotClient;
        }

        public async Task<ITelegramBotClient> GetRandomBotClient()
        {
            return _telegramBotClient;
        }
    }
}