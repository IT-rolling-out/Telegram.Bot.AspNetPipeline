namespace Telegram.Bot.AspNetPipeline.Core.ImprovedBot
{
    /// <summary>
    /// Used in bot exctensions to set which members messages must be processed.
    /// </summary>
    public enum ReadCallbackFromType
    {
        /// <summary>
        /// Only updates where Message.From is current user.
        /// </summary>
        CurrentUser=0,

        /// <summary>
        /// Only updates where Message.From is current user and message is reply to current bot.
        /// </summary>
        CurrentUserReply,

        /// <summary>
        /// Any reply messages to bot will be processed.
        /// </summary>
        AnyUserReply,

        /// <summary>
        /// Any message in chat will be processed.
        /// </summary>
        AnyUser
    }
}
