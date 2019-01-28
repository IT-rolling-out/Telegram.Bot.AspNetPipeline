using Microsoft.Extensions.DependencyInjection;

namespace Telegram.Bot.AspNetPipeline.Core.Builder
{
    /// <summary>
    /// Like IApplicationBuilder in asp.net .
    /// </summary>
    public interface IPipelineBuilder
    {
        /// <summary>
        /// Use it to configure your services.
        /// </summary>
        IServiceCollection Services { get; }

        void Use(UpdateProcessingDelegate middlewareDelegate);

        UpdateProcessingDelegate Build();
    }
}
