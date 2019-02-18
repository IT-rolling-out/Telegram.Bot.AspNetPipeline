using Telegram.Bot.AspNetPipeline.Mvc.Extensions.Main;

namespace Telegram.Bot.AspNetPipeline.Mvc.Extensions.MvcFeatures
{
    /// <summary>
    /// Implemented with <see cref="ServicesBus"/>.
    /// </summary>
    public interface IMvcFeaturesProvider
    {
        IMvcFeatures MvcFeatures { get; }
    }
}
