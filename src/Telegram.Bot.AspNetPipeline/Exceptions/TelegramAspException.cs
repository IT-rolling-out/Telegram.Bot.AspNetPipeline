using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Telegram.Bot.AspNetPipeline.Exceptions
{
    /// <summary>
    /// Used for general exceptions in library for it's fast identification.
    /// </summary>
    public class TelegramAspException : Exception
    {
        public TelegramAspException()
        {
        }

        public TelegramAspException(string message) : base(message)
        {
        }

        public TelegramAspException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TelegramAspException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
