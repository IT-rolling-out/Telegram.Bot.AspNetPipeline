using System;
using System.Threading;
using Telegram.Bot.AspNetPipeline.Extensions.Logging;

namespace Telegram.Bot.AspNetPipeline.Core.Internal
{
    /// <summary>
    /// Some data and services for main (mandatory) middleware.
    /// You can get it from UpdateContext.HiddenContext().
    /// <para></para>
    /// Please, dont use it without important reason.
    /// <para></para>
    /// Not really internal now. Was decided to allow the user to use HiddenUpdateContext.
    /// </summary>
    public class HiddenUpdateContext
    {
        /// <summary>
        /// You can access it from UpdateContext using following key.
        /// </summary>
        public const string DictKeyName = "_HiddenUpdateContext";

        public CancellationTokenSource UpdateProcessingAbortedSource { get; }

        public DateTime CreatedAt { get;  }

        /// <summary>
        /// More info in <see cref="LoggingAdvancedOptions"/> class summary.
        /// </summary>
        public LoggingAdvancedOptions LoggingAdvancedOptions { get; }

        public HiddenUpdateContext(CancellationTokenSource updateProcessingAbortedSource, DateTime createdAt, LoggingAdvancedOptions loggingAdvancedOptions)
        {
            UpdateProcessingAbortedSource = updateProcessingAbortedSource;
            CreatedAt = createdAt;
            LoggingAdvancedOptions = loggingAdvancedOptions;
        }

    }
}
