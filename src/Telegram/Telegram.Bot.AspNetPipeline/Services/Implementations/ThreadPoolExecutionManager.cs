using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using ConcurrentCollections;
using Telegram.Bot.AspNetPipeline.Extensions;

namespace Telegram.Bot.AspNetPipeline.Services.Implementations
{
    public class ThreadPoolExecutionManager : IExecutionManager
    {
        readonly ConcurrentHashSet<Task> _pendingTasks = new ConcurrentHashSet<Task>();

        public int PendingTasksCount => _pendingTasks.Count;

        /// <summary>
        /// Manage how delegate will be executed: in thread, in queue, synchronous or else.
        /// </summary>
        public async Task ProcessUpdate(Func<Task> func)
        {
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
        /// <returns>True if thread terminated because of timeout.</returns>
        public async Task<bool> AwaitAllPending(TimeSpan? timeout = null)
        {
            var res=await TaskExt.WhenAll(_pendingTasks, timeout);
            foreach (var t in _pendingTasks)
            {
                if (t.IsCanceled || t.IsCompleted || t.IsFaulted)
                {
                    _pendingTasks.TryRemove(t);
                }
            }
            return res;
        }


    }
}
