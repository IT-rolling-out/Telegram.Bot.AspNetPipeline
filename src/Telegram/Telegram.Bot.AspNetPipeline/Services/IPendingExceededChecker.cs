using System;
using System.Collections.Generic;
using System.Text;

namespace Telegram.Bot.AspNetPipeline.Core.Services
{
    public interface IPendingExceededChecker
    {
        bool IsPendingExceeded(UpdateContext updateContext);
    }
}
