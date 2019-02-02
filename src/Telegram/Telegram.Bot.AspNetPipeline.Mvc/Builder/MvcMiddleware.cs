using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Builder
{
    public class MvcMiddleware:IMiddleware
    {
        public MvcMiddleware(IAddMvcBuilder addMvcBuilder, IUseMvcBuilder useMvcBuilder)
        {
        }

        public Task Invoke(UpdateContext ctx, Func<Task> next)
        {
            throw new NotImplementedException();
        }
    }
}
