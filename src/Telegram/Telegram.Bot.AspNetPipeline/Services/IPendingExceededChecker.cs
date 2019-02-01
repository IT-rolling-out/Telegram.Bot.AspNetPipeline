using Telegram.Bot.AspNetPipeline.Core;

namespace Telegram.Bot.AspNetPipeline.Services
{
    public interface IPendingExceededChecker
    {
        bool IsPendingExceeded(UpdateContext updateContext);
    }
}
