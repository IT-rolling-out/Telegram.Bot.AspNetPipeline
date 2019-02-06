using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using ConcurrentCollections;
using IRO.Common.Services;
using Microsoft.Extensions.Logging;

namespace Telegram.Bot.AspNetPipeline.Services
{
    public class ThreadPoolExecutionManager : IExecutionManager
    {
        readonly ILogger _logger;

        readonly ConcurrentHashSet<Task> _pendingTasks = new ConcurrentHashSet<Task>();

        public int PendingTasksCount => _pendingTasks.Count;

        public ThreadPoolExecutionManager(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(GetType());
        }

        /// <summary>
        /// Manage how delegate will be executed: in thread, in queue, synchronous or else.
        /// </summary>
        public async Task ProcessUpdate(Func<Task> func)
        {
            Task resTask = null;
            try
            {
                //!If we await default Task.Run(func) - exception will not be shown in VS.
                //But even with current crunch it showed only here, not on called method.
                ExceptionDispatchInfo exceptionDispatchInfo = null;
                await Task.Run(async ()=>
                {
                    try
                    {
                        resTask= func.Invoke();
                    }
                    catch(Exception ex)
                    {
                        exceptionDispatchInfo = ExceptionDispatchInfo.Capture(ex);
                    }
                });
                _pendingTasks.Add(resTask);
                await resTask.ContinueWith((t) =>
                {
                    if (exceptionDispatchInfo != null)
                    {
                        Task.Run(() =>
                        {
                            _logger.LogError("Exception in executed delegate '{0}'",
                                exceptionDispatchInfo.SourceException);
                            exceptionDispatchInfo.Throw();
                        });
                    }
                });
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
            var res = await TaskExt.WhenAll(_pendingTasks, timeout);
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
