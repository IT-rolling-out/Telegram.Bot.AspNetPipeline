namespace Telegram.Bot.AspNetPipeline.Core.BotExt
{
    /// <summary>
    /// Used in bot exctensions to set which members messages must be processed.
    /// </summary>
    public enum ReadCallbackFromType
    {
        /// <summary>
        /// Any UpdateContext.Message.From messages can be returned from callback.
        /// </summary>
        CurrentUser=0,

        /// <summary>
        /// Only UpdateContext.Message.From replies can be returned from callback.
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
