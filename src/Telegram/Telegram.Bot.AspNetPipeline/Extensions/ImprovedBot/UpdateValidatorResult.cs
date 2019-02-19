namespace Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot
{
    public enum UpdateValidatorResult
    {
        Valid,

        /// <summary>
        /// Not valid, but continue waiting
        /// </summary>
        ContinueWaiting,

        /// <summary>
        /// Abort ReadMessageAsync thread.
        /// </summary>
        AbortWaiter
    }
}