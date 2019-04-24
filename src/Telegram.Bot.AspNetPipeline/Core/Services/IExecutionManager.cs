using System;
using System.Threading.Tasks;

namespace Telegram.Bot.AspNetPipeline.Core.Services
{
    /// <summary>
    /// Manage how delegate will be executed: in thread, in queue, synchronous or else.
    /// </summary>
    public interface IExecutionManager
    {
        int PendingTasksCount { get; }

        /// <summary>
        /// Manage how delegate will be executed: in thread, in queue, synchronous or else.
        /// </summary>
        Task ProcessUpdate(Func<Task> func);

        /// <summary>
        /// Wait all tasks completition with try|cathc block.
        /// </summary>
        /// <returns>True if thread terminated because of timeout.</returns>
        Task<bool> AwaitAllPending(TimeSpan? timeout = null);
    }
}
