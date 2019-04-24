using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Telegram.Bot.AspNetPipeline.Builder
{
    /// <summary>
    /// Just wrapper used to separate ASP.NET extensions and extensions in Telegram.Bot.AspNetPipeline library.
    /// </summary>
    public class ServiceCollectionWrapper
    {
        public IServiceCollection Services { get; } 

        public ServiceCollectionWrapper(IServiceCollection services)
        {
            Services = services;
        }
    }
}
