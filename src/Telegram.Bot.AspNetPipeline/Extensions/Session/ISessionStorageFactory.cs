using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Telegram.Bot.AspNetPipeline.Exceptions;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Extensions.Session
{
    public interface ISessionStorageFactory
    {
        ISessionStorage ResolveSessionStorage(string chatUsername);
    }

    public class RamSessionStorageFactory : ISessionStorageFactory
    {
        readonly TimeSpan _sessionTimeout;

        readonly IDictionary<string, (ISessionStorage Session, DateTime LastAccess)> _sessionsDict =
            new ConcurrentDictionary<string, (ISessionStorage, DateTime)>();

        /// <summary>
        /// Sessions, that wasn't resolved for a given time will be removed.
        /// Note: <see cref="ISessionStorage"/> must be resolved, if you want update last access time.
        /// Last access time will not be updated automatically on each message from chat.
        /// </summary>
        /// <param name="sessionTimeout"></param>
        public RamSessionStorageFactory(TimeSpan sessionTimeout)
        {
            _sessionTimeout = sessionTimeout;
            var t = new Timer(10000);
            var weakThis=new WeakReference<RamSessionStorageFactory>(this);
            t.Elapsed += (s, e) =>
            {
                weakThis.RemoveTimeouted();

            };
            t.Start();
        }

        public ISessionStorage ResolveSessionStorage(string chatUsername)
        {
            if (string.IsNullOrWhiteSpace(chatUsername))
                throw new ArgumentException("Can't be null or white space.", nameof(chatUsername));

            ISessionStorage session;
            if (_sessionsDict.TryGetValue(chatUsername, out var val))
            {
                session = val.Session;
            }
            else
            {
                session = new RamSessionStorage(chatUsername);
            }
            _sessionsDict[chatUsername] = (session, DateTime.Now);
            RemoveTimeouted();
            return session;

        }

        #region Sessions timeout.
        readonly TimeSpan _checkTimeout = TimeSpan.FromMinutes(5);

        DateTime _lastCheck = DateTime.MinValue;

        readonly object _checkLocker = new object();

        void RemoveTimeouted()
        {
            if (DateTime.Now - _lastCheck < _checkTimeout)
                return;

            Task.Run(() =>
            {
                lock (_checkLocker)
                {
                    if (DateTime.Now - _lastCheck < _checkTimeout)
                        return;
                    var sessionsCopy = _sessionsDict.ToArray();
                    foreach (var pair in sessionsCopy)
                    {
                        var mustBeRemoved = DateTime.Now - _sessionTimeout > pair.Value.LastAccess;
                    }

                    _lastCheck = DateTime.Now;

                }
            });
        }
        #endregion
    }
}
