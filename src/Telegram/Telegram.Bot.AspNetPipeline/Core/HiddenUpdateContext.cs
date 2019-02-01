using System;
using System.Threading;

namespace Telegram.Bot.AspNetPipeline.Core
{
    /// <summary>
    /// Some data and services for main (mandatory) middleware.
    /// You can get it from UpdateContext.Properties["HiddenUpdateContext"].
    /// <para></para>
    /// Please, dont use it without important reason.
    /// </summary>
    public class HiddenUpdateContext
    {
        /// <summary>
        /// You can access it from UpdateContext using following key.
        /// </summary>
        public const string DictKeyName = "HiddenUpdateContext";

        public CancellationTokenSource UpdateProcessingAbortedSource { get; }

        public DateTime CreatedAt { get;  }

        public HiddenUpdateContext(CancellationTokenSource updateProcessingAbortedSource, DateTime createdAt)
        {
            UpdateProcessingAbortedSource = updateProcessingAbortedSource;
            CreatedAt = createdAt;
        }

        public static HiddenUpdateContext Resolve(UpdateContext updateContext)
        {
            return (HiddenUpdateContext)updateContext.Properties[HiddenUpdateContext.DictKeyName];
        }
    }
}
