using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Core.Services
{
    public interface ISession
    {
        User User { get; }

        /// <summary>
        /// Return value or null.
        /// </summary>
        /// <param name="type">Type of returned object.</param>
        Task<object> GetOrNull(Type type, string key);

        /// <summary>
        /// If value is 'null', then method will remove that value from the cache.
        /// </summary>
        Task Set(string key, object value);

        Task Clear();

    }

}