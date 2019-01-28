using System;
using System.Threading.Tasks;

namespace Telegram.Bot.AspNetPipeline.Core.Builder
{
    public interface IMiddleware
    {
        Task Invoke(UpdateContext ctx, Func<Task> next);
    }
}
