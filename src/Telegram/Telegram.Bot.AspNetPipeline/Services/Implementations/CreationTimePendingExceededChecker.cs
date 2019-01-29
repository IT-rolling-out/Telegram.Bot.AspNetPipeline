using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Core.Services;

namespace Telegram.Bot.AspNetPipeline.Implementations
{
    public class CreationTimePendingExceededChecker : IPendingExceededChecker
    {
        public TimeSpan PendingTimeLimit { get; }

        public CreationTimePendingExceededChecker(TimeSpan pendingTimeLimit)
        {
            PendingTimeLimit = pendingTimeLimit;
        }
        
        /// <summary>
        /// Return true if DateTime.Now - {UpdContextCreationTime} > PendingTimeLimit.
        /// </summary>
        public bool IsPendingExceeded(UpdateContext updateContext)
        {
            var hiddenCtx = (HiddenUpdateContext)updateContext.Properties[HiddenUpdateContext.DictKeyName];
            return DateTime.Now - hiddenCtx.CreatedAt > PendingTimeLimit;
        }
    }
}
