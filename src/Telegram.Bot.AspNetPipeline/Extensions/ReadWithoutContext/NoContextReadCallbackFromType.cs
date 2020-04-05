namespace Telegram.Bot.AspNetPipeline.Extensions.ReadWithoutContext
{
    public enum NoContextReadCallbackFromType
    {
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