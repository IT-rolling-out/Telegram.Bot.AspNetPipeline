using System;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Core.Builder;

namespace Telegram.Bot.AspNetPipeline.Core.ImprovedBot
{
    internal class ImprovedBotMiddleware : IMiddleware
    {
        public Task Invoke(UpdateContext ctx, Func<Task> next)
        {
            throw new NotImplementedException();
        }
    }
}
