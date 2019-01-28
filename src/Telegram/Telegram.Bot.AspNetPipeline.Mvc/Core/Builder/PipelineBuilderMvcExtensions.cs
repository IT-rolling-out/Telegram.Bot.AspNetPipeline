using System;
using Telegram.Bot.AspNetPipeline.Core.Builder;

namespace Telegram.Bot.AspNetPipeline.Mvc.Core.Builder
{
    public static class PipelineBuilderMvcExtensions
    {
        public static void UseMvc(this IPipelineBuilder pipelineBuilder, Action<IBotMvcBuilder> act=null)
        {
        }
    }
}
