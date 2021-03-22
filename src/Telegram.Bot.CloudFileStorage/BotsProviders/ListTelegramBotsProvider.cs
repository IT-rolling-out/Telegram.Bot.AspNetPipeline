using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.CloudFileStorage.Data;

namespace Telegram.Bot.CloudFileStorage.BotsProviders
{
    public class ListTelegramBotsProvider : ITelegramBotsProvider
    {
        readonly Func<string, ITelegramBotClient> _botClientFactory;
        readonly Random _random = new Random();
        readonly IDictionary<long, TelegramBotInfo> _botsInfoById = new ConcurrentDictionary<long, TelegramBotInfo>();
        readonly IDictionary<long, ITelegramBotClient> _botsClientsById = new ConcurrentDictionary<long, ITelegramBotClient>();

        /// <summary>
        /// You can write your own implemention with <see cref="GetBotInfo"/> integrated with database or something.
        /// </summary>
        public ListTelegramBotsProvider(Func<string, ITelegramBotClient> botClientFactory, IEnumerable<TelegramBotInfo> predefinedBots)
        {
            _botClientFactory = botClientFactory ?? throw new ArgumentNullException(nameof(botClientFactory));
            foreach (var botInfo in predefinedBots)
            {
                _botsInfoById.Add(botInfo.BotId, botInfo);
            }
        }
        
        public async Task<ITelegramBotClient> GetBotClient(long id)
        {
            if (!_botsClientsById.ContainsKey(id))
            {
                var botInfo = GetBotInfo(id);
                _botsClientsById[id] = _botClientFactory(botInfo.Token);
            }
            return _botsClientsById[id];
        }

        public Task<ITelegramBotClient> GetRandomBotClient()
        {
            var list=_botsInfoById.ToList();
            var botInfo = list[_random.Next(list.Count)];
            return GetBotClient(botInfo.Value.BotId);
        }

        TelegramBotInfo GetBotInfo(long id)
        {
            return _botsInfoById[id];
        }
    }
}
