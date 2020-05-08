using System;
using System.Collections.Concurrent;
using System.Timers;

namespace Telegram.Bot.AspNetPipeline.Extensions.Session
{
    public class RamSessionStorageProvider : ISessionStorageProvider, IDisposable
    {
        Timer _timer;

        readonly TimeSpan _sessionTimeout;

        readonly ConcurrentDictionary<string, (ISessionStorage Session, DateTime LastAccess)> _sessionsDict =
            new ConcurrentDictionary<string, (ISessionStorage, DateTime)>();

        /// <summary>
        /// Sessions, that wasn't resolved for a given time will be removed.
        /// Note: <see cref="ISessionStorage"/> must be resolved, if you want update last access time.
        /// Last access time will not be updated automatically on each message from chat.
        /// </summary>
        /// <param name="sessionTimeout"></param>
        /// /// <param name="checkInterval">
        /// Indicate how often it must check and remove old sessions.
        /// Default is 5 minutes.
        /// </param>
        public RamSessionStorageProvider(TimeSpan? sessionTimeout, TimeSpan? checkInterval = null)
        {
            _sessionTimeout = sessionTimeout ?? TimeSpan.FromMinutes(30);
            var interval = checkInterval ?? TimeSpan.FromMinutes(5);

            //Start timer.
            _timer = new Timer(interval.TotalMilliseconds);
            var lastCheck = DateTime.MinValue;
            _timer.Elapsed += (s, e) =>
            {
                //Skip if previous wasn't finished.
                if (DateTime.Now - interval < lastCheck)
                    return;
                RemoveTimeouted();
                lastCheck = DateTime.Now;
            };
            _timer.Start();
        }

        public ISessionStorage ResolveSessionStorage(long botId, long chatId)
        {
            var key = $"{botId}_{chatId}";
            ISessionStorage session;
            if (_sessionsDict.TryGetValue(key, out var val))
            {
                session = val.Session;
            }
            else
            {
                session = new RamSessionStorage(chatId);
            }
            _sessionsDict[key] = (session, DateTime.Now);
            return session;

        }

        #region Sessions timeout.
        readonly object _checkLocker = new object();

        void RemoveTimeouted()
        {
            lock (_checkLocker)
            {
                var sessionsCopy = _sessionsDict.ToArray();
                foreach (var pair in sessionsCopy)
                {
                    var mustBeRemoved = DateTime.Now - _sessionTimeout > pair.Value.LastAccess;
                    if (mustBeRemoved)
                    {
                        _sessionsDict.TryRemove(pair.Key, out var ignored);
                    }
                }
            }
        }
        #endregion

        public void Dispose()
        {
            _timer?.Dispose();
            _timer = null;
        }
    }
}
