using System;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Core;

namespace Telegram.Bot.AspNetPipeline.Builder
{
    public interface IMiddleware
    {
        Task Invoke(UpdateContext ctx, Func<Task> next);
    }
}
