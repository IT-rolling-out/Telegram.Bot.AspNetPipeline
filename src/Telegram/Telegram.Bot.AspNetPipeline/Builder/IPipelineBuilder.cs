using System;
using Telegram.Bot.AspNetPipeline.Core;

namespace Telegram.Bot.AspNetPipeline.Builder
{
    /// <summary>
    /// Like IApplicationBuilder in asp.net .
    /// </summary>
    public interface IPipelineBuilder
    {
        IServiceProvider ServiceProvider { get; }

        void Use(UpdateProcessingDelegate middlewareDelegate);

        UpdateProcessingDelegate Build();
    }
}
