using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using ConcurrentCollections;

namespace Telegram.Bot.AspNetPipeline.Services.Implementations
{
    public class ThreadPoolExecutionManager:IExecutionManager
    {
        readonly object _locker = new object();

        readonly ConcurrentHashSet<Task> _pendingTasks = new ConcurrentHashSet<Task>();

        readonly Mutex _mutex = new Mutex();

        public int PendingTasksCount => _pendingTasks.Count;

        /// <summary>
        /// Manage how delegate will be executed: in thread, in queue, synchronous or else.
        /// </summary>
        public async Task ProcessUpdate(Func<Task> func)
        {
            _mutex.WaitOne();
            _mutex.ReleaseMutex();
            Task resTask = null;
            try
            {
                resTask = Task.Run(func);
                _pendingTasks.Add(resTask);
                await resTask;
            }
            finally
            {
                if (resTask != null)
                {
                    _pendingTasks.TryRemove(resTask);
                }
            }
        }

        /// <summary>
        /// Wait all tasks completition with try|cathc block.
        /// </summary>
        public async Task AwaitAllPending()
        {
            try
            {
                _mutex.WaitOne();
                foreach (var t in _pendingTasks)
                {
                    try
                    {
                        await t;
                    }
                    catch { }
                }
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }


    }
}
