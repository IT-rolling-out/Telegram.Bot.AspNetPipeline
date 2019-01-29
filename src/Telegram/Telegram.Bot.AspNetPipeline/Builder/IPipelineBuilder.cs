using System;
using Microsoft.Extensions.DependencyInjection;

namespace Telegram.Bot.AspNetPipeline.Core.Builder
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
