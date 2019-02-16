namespace Telegram.Bot.AspNetPipeline.Mvc.Extensions.Main
{
    /// <summary>
    /// Implemented with <see cref="ServicesBus"/>.
    /// </summary>
    public interface IMvcFeaturesProvider
    {
        IMvcFeatures MvcFeatures { get; }
    }
}
