using System;
using System.Threading.Tasks;

namespace Telegram.Bot.AspNetPipeline.Services
{
    /// <summary>
    /// Manage how delegate will be executed: in thread, in queue, synchronous or else.
    /// </summary>
    public interface IExecutionManager
    {
        /// <summary>
        /// Manage how delegate will be executed: in thread, in queue, synchronous or else.
        /// </summary>
        Task ProcessUpdate(Func<Task> func);
    }
}
