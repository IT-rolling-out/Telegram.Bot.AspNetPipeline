﻿using System;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Builder;

namespace Telegram.Bot.AspNetPipeline.Mvc.Builder
{
    public static class PipelineBuilderMvcExtensions
    {
        public static void UseMvc(this IPipelineBuilder pipelineBuilder, Action<IMvcBuilder> mvcBuilderDelegate = null)
        {
            throw new NotImplementedException();
        }

        public static void AddMvc(this IServiceCollection @this, Action<IMvcServicesRegistrator> controllersConfigureDelegate=null)
        {
            throw new NotImplementedException();
        }
    }
}