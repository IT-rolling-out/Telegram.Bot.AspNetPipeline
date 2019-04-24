using System;
using System.Text;
using System.Threading.Tasks;
using IRO.Cache;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Extensions.Session
{
    public interface ISessionStorage
    {
        ChatId ChatId { get; }

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
