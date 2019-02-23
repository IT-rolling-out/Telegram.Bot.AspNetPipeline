namespace Telegram.Bot.AspNetPipeline.Core.Services
{
    public interface IPendingExceededChecker
    {
        bool IsPendingExceeded(UpdateContext updateContext);
    }
}
