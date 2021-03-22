using System.Threading.Tasks;
using Telegram.Bot.CloudFileStorage.Data;

namespace Telegram.Bot.CloudFileStorage
{
    /// <summary>
    /// Current interface designed to manage services which use multiple bots.
    /// </summary>
    public interface ITelegramBotsProvider
    {
        Task<ITelegramBotClient> GetBotClient(long id);

        Task<ITelegramBotClient> GetRandomBotClient();
    }

}