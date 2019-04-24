using System;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Core;

namespace Telegram.Bot.AspNetPipeline.Extensions.ExceptionHandling
{
    /// <summary>
    /// </summary>
    /// <returns>False to throw exception.</returns>
    public delegate Task<bool> UpdateProcessingExceptionDelegate(UpdateContext ctx, Exception ex);
}