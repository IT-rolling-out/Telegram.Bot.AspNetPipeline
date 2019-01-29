using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Telegram.Bot.AspNetPipeline.Core
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
