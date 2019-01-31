using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Core
{
    public interface IUpdateContextSearchBag
    {
        /// <summary>
        /// If context with current key exists - it will be disposed and overridden by current context.
        /// </summary>
        void Add(UpdateContextSearchData searchData);

        /// <summary>
        /// Return value or null.
        /// </summary>
        UpdateContextSearchData? TryFind(long chatId, int botId);

        bool Contains(long chatId, int botId);
    }
}