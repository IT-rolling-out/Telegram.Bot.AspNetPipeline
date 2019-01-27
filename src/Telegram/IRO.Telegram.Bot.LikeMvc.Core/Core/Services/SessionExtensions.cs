using System.Threading.Tasks;

namespace IRO.Telegram.Bot.ProcessingPipeline.Core.Services
{
    public static class SessionExtensions
    {
        /// <summary>
        /// Just call of Set(key, null).
        /// </summary>
        public static async Task Remove(this ISession @this, string key)
        {
            await @this.Set(key, null);
        }

        /// <summary>
        /// Return value or null.
        /// </summary>
        public static async Task<T> GetOrNull<T>(this ISession @this, string key)
            where T : class
        {
            object value = await @this.GetOrNull(typeof(T), key);
            if (value is T valueConverted)
            {
                return valueConverted;
            }
            return null;
        }

        /// <summary>
        /// Return value or default.
        /// Use nullable for value types.
        /// </summary>
        public static async Task<T> GetOrDefault<T>(this ISession @this, string key)
        {
            object value = await @this.GetOrNull(typeof(T), key);
            if (value is T valueConverted)
            {
                return valueConverted;
            }
            return default(T);
        }

    }
}
