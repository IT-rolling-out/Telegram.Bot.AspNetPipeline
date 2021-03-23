using System.Threading.Tasks;
using IRO.Storage;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Metadata;
using Telegram.Bot.CloudFileStorage;

namespace IRO.Tests.Telegram.Controllers
{
    public class TgStorageTestController : BotController
    {
        readonly TelegramStorage _telegramStorage;

        public TgStorageTestController(TelegramStorage telegramStorage)
        {
            _telegramStorage = telegramStorage;
        }

        [BotRoute("/st_set")]
        public async Task Set(string str)
        {
            await _telegramStorage.Set("myVal", str);
            await SendTextMessageAsync($"Setted.");
        }

        [BotRoute("/st_get")]
        public async Task Get()
        {
            var str = await _telegramStorage.GetOrDefault<string>("myVal");
            await SendTextMessageAsync(str ?? "null");
        }

        [BotRoute("/force_load")]
        public async Task ForceLoad()
        {
            await _telegramStorage.ForceLoad();
            await SendTextMessageAsync($"Loaded.");
        }

        [BotRoute("/force_save")]
        public async Task ForceSave()
        {
            await _telegramStorage.ForceSave();
            await SendTextMessageAsync("Saved.");
        }

    }
}