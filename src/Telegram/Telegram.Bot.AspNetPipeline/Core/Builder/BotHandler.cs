using System;
using Microsoft.Extensions.DependencyInjection;

namespace Telegram.Bot.AspNetPipeline.Core.Builder
{
    /// <summary>
    /// Like IWebHostBuilder in asp.net .
    /// </summary>
    public class BotHandler
    {
        public IServiceCollection Services { get; }

        public ITelegramBotClient Bot { get; }

        public BotHandler(ITelegramBotClient bot, IServiceCollection servicesCollection = null)
        {
            Bot = bot;
            Services = servicesCollection ?? new ServiceCollection();
        }

        public void ConfigureBuilder(Action<IPipelineBuilder> action)
        {

        }

        public void Start()
        {

        }

        public void Stop()
        {

        }
    }
}
