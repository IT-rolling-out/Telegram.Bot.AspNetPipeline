using System;
using System.Threading;
using System.Threading.Tasks;

namespace Telegram.Bot.AspNetPipeline.Services.Implementations
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
