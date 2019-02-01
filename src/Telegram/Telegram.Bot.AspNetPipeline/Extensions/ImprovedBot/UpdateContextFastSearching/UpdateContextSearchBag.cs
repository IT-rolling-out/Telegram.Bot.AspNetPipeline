using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot.UpdateContextFastSearching
{
    /// <summary>
    /// Specialized only for "ImprovedBot".
    /// </summary>
    class UpdateContextSearchBag : IUpdateContextSearchBag
    {
        readonly ConcurrentDictionary<string, UpdateContextSearchData> _dict = new ConcurrentDictionary<string, UpdateContextSearchData>();

        /// <summary>
        /// Return value or null.
        /// </summary>
        public UpdateContextSearchData? TryFind(long chatId, int botId)
        {
            var key = UpdateContextSearchData.CreateKey(chatId, botId);
            if (_dict.TryGetValue(key, out var val))
            {
                return val;
            }
            else
            {
                return null;
            }
        }

        public bool Contains(long chatId, int botId)
        {
            var key = UpdateContextSearchData.CreateKey(chatId, botId);
            return _dict.ContainsKey(key);
        }

        /// <summary>
        /// If context with current key exists - it will be disposed and overridden by current context.
        /// </summary>
        public void Add(UpdateContextSearchData searchData)
        {
            RemoveDisposed();
            _dict[searchData.Key] = searchData;
        }

        #region Remove disposed.
        readonly TimeSpan _checkDisposedDelay = TimeSpan.FromSeconds(30);

        readonly object _checkDisposedLocker = new object();

        DateTime _lastDisposedCheck = DateTime.Now;

        void RemoveDisposed()
        {
            if (DateTime.Now - _lastDisposedCheck < _checkDisposedDelay)
            {
                return;
            }

            Task.Run(() =>
            {
                lock (_checkDisposedLocker)
                {
                    if (DateTime.Now - _lastDisposedCheck < _checkDisposedDelay)
                    {
                        return;
                    }

                    foreach (var item in _dict)
                    {
                        if (item.Value.CurrentUpdateContext.IsDisposed)
                        {
                            _dict.TryRemove(item.Key, out var val);
                        }
                    }

                    _lastDisposedCheck = DateTime.Now;
                }
            });
        }
        #endregion
    }

}
