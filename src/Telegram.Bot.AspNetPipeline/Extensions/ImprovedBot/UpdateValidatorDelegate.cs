using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot
{
    /// <summary>
    /// </summary>
    /// <param name="newUpdateContext">New message from current chat. Passed before middlewares will process it.</param>
    /// <param name="originalUpdateContext">Update context of waiting method (where ReadMessageAsync invoked).</param>
    public delegate Task<UpdateValidatorResult> UpdateValidatorDelegate(UpdateContext newUpdateContext, UpdateContext originalUpdateContext);
}
