using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Telegram.Bot.AspNetPipeline.Core.Exceptions
{
    public class UpdateProcessingException : Exception
    {
        public UpdateProcessingException()
        {
        }

        public UpdateProcessingException(string message) : base(message)
        {
        }

        public UpdateProcessingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UpdateProcessingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
