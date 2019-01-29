using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Core.Builder;

namespace Telegram.Bot.AspNetPipeline.Core.BotExt
{
    internal static class BotExtPipelineBuilderExtensions
    {
        public static void UseBotExt(this IPipelineBuilder @this)
        {
            @this.UseMiddlware(new BotExtMiddleware());
        }
    }

    internal class BotExtMiddleware : IMiddleware
    {
        public Task Invoke(UpdateContext ctx, Func<Task> next)
        {
            throw new NotImplementedException();
        }
    }
}
