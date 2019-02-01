using System;
using System.Threading.Tasks;

namespace Telegram.Bot.AspNetPipeline.Core
{
    public delegate Task UpdateProcessingDelegate(UpdateContext ctx, Func<Task> next);
}
