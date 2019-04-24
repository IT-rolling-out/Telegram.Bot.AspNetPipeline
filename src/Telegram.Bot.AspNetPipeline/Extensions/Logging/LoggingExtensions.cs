using System;
using Microsoft.Extensions.Logging;

namespace Telegram.Bot.AspNetPipeline.Extensions.Logging
{
    public static class LoggingExtensions
    {
        /// <summary>
        /// All objects from array will be used in BeginScope.
        /// <para></para>
        /// Parameter in action is current logger.
        /// </summary>
        public static void MultipleScope(this ILogger @this, Action<ILogger> action, params object[] scopeStates)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            var scopeStatesWrappers = new IDisposable[scopeStates.Length];
            try
            {
                for (int i = 0; i < scopeStatesWrappers.Length; i++)
                {
                    scopeStatesWrappers[i] = @this.BeginScope(scopeStates[i]);
                }
                action.Invoke(@this);
            }
            finally
            {
                foreach (var item in scopeStatesWrappers)
                {
                    item?.Dispose();
                }
            }

        }

        /// <summary>
        /// All objects from array will be used in BeginScope.
        /// <para></para>
        /// Same as overload method, but without passed to delegate logger.
        /// </summary>
        public static void MultipleScope(this ILogger @this, Action action, params object[] scopeStates)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            @this.MultipleScope((logger) => action.Invoke());
        }
    }
}
