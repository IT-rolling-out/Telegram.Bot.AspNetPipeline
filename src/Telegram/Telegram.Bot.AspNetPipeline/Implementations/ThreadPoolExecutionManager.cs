using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Core;

namespace Telegram.Bot.AspNetPipeline.Implementations
{
    public class ThreadPoolExecutionManager:IExecutionManager
    {
        /// <summary>
        /// Manage how delegate will be executed: in thread, in queue, synchronous or else.
        /// </summary>
        public void ProcessUpdate(Func<Task> func)
        {
            ThreadPool.QueueUserWorkItem((s) =>
            {
                func.Invoke();
            });
        }
    }
}
