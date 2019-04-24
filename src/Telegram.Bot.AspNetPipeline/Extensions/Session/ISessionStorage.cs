using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Extensions.Session
{
    public interface ISessionStorage
    {
        long ChatId { get; }

        IEnumerable<string> Keys { get; }

        /// <summary>
        /// Null works as remove.
        /// </summary>
        Task Set(string key, object value);

        /// <summary>
        /// Return value or throws exception.
        /// </summary>
        Task<object> Get(Type t, string key);

        void Clear();
    }
}
