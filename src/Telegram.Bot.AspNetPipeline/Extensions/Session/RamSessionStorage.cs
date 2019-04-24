using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Telegram.Bot.AspNetPipeline.Exceptions;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Extensions.Session
{
    public class RamSessionStorage:ISessionStorage
    {
        readonly IDictionary<string, string> _dict = new ConcurrentDictionary<string, string>();

        public string ChatUsername { get; }

        public RamSessionStorage(string chatUsername)
        {
            ChatUsername = chatUsername;
        }

        /// <summary>
        /// Null works as remove.
        /// </summary>
        public async Task Set(string key, object value)
        {
            if (value == null)
            {
                _dict.Remove(key);
                return;
            }
            var serializedVal = JsonConvert.SerializeObject(value);
            _dict[key]=serializedVal;
        }

        /// <summary>
        /// Return value or throws exception.
        /// </summary>
        public async Task<object> Get(Type t, string key)
        {
            if (_dict.TryGetValue(key, out var serializedVal))
            {
                var value = JsonConvert.DeserializeObject(serializedVal, t);
                return value;
            }
            else
            {
                throw new TelegramAspException($"Can't find session value for key '{key}'.");
            }
        }

        public void Clear()
        {
            _dict.Clear();
        }
    }
}
