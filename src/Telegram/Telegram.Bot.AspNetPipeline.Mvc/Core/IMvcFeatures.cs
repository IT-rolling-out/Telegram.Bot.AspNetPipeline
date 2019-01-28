namespace Telegram.Bot.AspNetPipeline.Mvc.Core
{
    /// <summary>
    /// Some other mvc features.
    /// </summary>
    public interface IMvcFeatures
    {
        /// <summary>
        /// Started another controller action when current action execution finished.
        /// Just pass current UpdateContext to another action.
        /// </summary>
        /// <param name="name">Name property from BotRouteAttribute.</param>
        void StartAnotherAction(string name);
    }
}
