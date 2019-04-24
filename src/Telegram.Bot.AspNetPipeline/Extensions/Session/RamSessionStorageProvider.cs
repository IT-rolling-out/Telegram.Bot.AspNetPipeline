using System;
using System.Collections.Concurrent;
using System.Timers;

namespace Telegram.Bot.AspNetPipeline.Extensions.Session
{
    public class RamSessionStorageProvider : ISessionStorageProvider
    {
        readonly TimeSpan _sessionTimeout;

        readonly ConcurrentDictionary<long, (ISessionStorage Session, DateTime LastAccess)> _sessionsDict =
            new ConcurrentDictionary<long, (ISessionStorage, DateTime)>();

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

            interval = TimeSpan.FromSeconds(3);

            //Start timer.
            var timer = new Timer(interval.TotalMilliseconds);
            //Use WeakReference to stop timer when all references to this was removed.
            var weakThis = new WeakReference<RamSessionStorageProvider>(this);
            var lastCheck = DateTime.MinValue;
            timer.Elapsed += (s, e) =>
            {
                if (weakThis.TryGetTarget(out var target))
                {
                    //Skip if previous wasn't finished.
                    if (DateTime.Now - interval < lastCheck)
                        return;
                    target.RemoveTimeouted();
                    lastCheck = DateTime.Now;
                }
                else
                {
                    timer.Dispose();
                }
            };
            timer.Start();
        }

        public ISessionStorage ResolveSessionStorage(long chatId)
        {
            ISessionStorage session;
            if (_sessionsDict.TryGetValue(chatId, out var val))
            {
                session = val.Session;
            }
            else
            {
                session = new RamSessionStorage(chatId);
            }
            _sessionsDict[chatId] = (session, DateTime.Now);
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
    }
}
