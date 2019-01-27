using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IRO.Telegram.Bot.ProcessingPipeline.Core
{
    public interface IMiddleware
    {
        Task Invoke(UpdateContext ctx, Func<Task> next);
    }
}
