namespace Telegram.Bot.AspNetPipeline.Core
{
    /// <summary>
    /// Not really internal now. Was decided to allow the user to use HiddenUpdateContext.
    /// </summary>
    public static class HiddenUpdateContextExtensions
    {
        public static HiddenUpdateContext HiddenContext(this UpdateContext @this)
        {
            return (HiddenUpdateContext)@this.Properties[HiddenUpdateContext.DictKeyName];
        }
    }
}
