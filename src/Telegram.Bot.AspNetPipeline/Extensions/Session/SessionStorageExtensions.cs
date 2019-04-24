using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Extensions.Session;

//ReSharper disable CheckNamespace
namespace Telegram.Bot.AspNetPipeline.Extensions
{
    public static class SessionStorageExtensions
    {
        /// <summary>
        /// Return value or throws exception.
        /// </summary>
        public static async Task<TValue> Get<TValue>(this ISessionStorage @this, string key)
        {
            var res= await @this.Get(typeof(TValue), key);
            return (TValue)res;
        }

        public static async Task<TValue> GetOrDefault<TValue>(this ISessionStorage @this, string key)
        {
            try
            {
                return await @this.Get<TValue>(key);
            }
            catch
            {
                return default(TValue);
            }
        }

    }
}
