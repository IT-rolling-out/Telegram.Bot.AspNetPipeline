using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace IRO.Telegram.Bot.ProcessingPipeline.Core
{
    public delegate Task MiddlewareDelegate(UpdateContext ctx, Func<Task> next);

    public delegate Task InitialMiddlewareDelegate(Update update, Func<Task> next);
}
