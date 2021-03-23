using System.Threading.Tasks;

namespace Telegram.Bot.CloudFileStorage.BotsProviders
{
    /// <summary>
    /// Current interface designed to manage services which use multiple bots.
    /// </summary>
    public interface ITelegramBotsProvider
    {
        Task<ITelegramBotClient> GetMainBotClient();

        Task<ITelegramBotClient> GetBotClient(long id);

        Task<ITelegramBotClient> GetRandomBotClient();
    }

}