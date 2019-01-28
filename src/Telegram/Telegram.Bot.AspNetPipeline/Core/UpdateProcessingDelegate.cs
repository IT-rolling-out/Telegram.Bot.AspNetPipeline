using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Core
{
    public delegate Task UpdateProcessingDelegate(UpdateContext ctx, Func<Task> next);
}
