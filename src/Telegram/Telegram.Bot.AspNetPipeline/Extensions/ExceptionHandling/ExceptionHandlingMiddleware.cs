using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Core;

namespace Telegram.Bot.AspNetPipeline.Extensions.ExceptionHandler
{
    internal class ExceptionHandlingMiddleware:IMiddleware
    {
        public IList<UpdateProcessingExceptionDelegate> ExceptionHandlers { get; } = new List<UpdateProcessingExceptionDelegate>();

        public async Task Invoke(UpdateContext ctx, Func<Task> next)
        {
            try
            {
                await next();
            }
            catch (Exception ex)
            {
                if (ex is TaskCanceledException)
                    return;
                var handled = false;
                foreach (var handler in ExceptionHandlers)
                {
                    handled = await handler(ctx, ex);
                    if (handled)
                    {
                        break;
                    }
                }

                if (!handled)
                    throw;
            }
        }
    }
}
