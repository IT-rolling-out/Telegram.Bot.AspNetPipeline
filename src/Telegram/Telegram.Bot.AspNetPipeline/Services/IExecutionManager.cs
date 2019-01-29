using System;
using System.Threading.Tasks;

namespace Telegram.Bot.AspNetPipeline.Core.Services
{
    /// <summary>
    /// Manage how delegate will be executed: in thread, in queue, synchronous or else.
    /// </summary>
    public interface IExecutionManager
    {
        /// <summary>
        /// Manage how delegate will be executed: in thread, in queue, synchronous or else.
        /// </summary>
        void ProcessUpdate(Func<Task> func);
    }
}
