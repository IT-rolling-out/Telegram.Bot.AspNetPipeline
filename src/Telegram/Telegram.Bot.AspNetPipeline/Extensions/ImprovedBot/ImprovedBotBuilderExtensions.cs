using System;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot.UpdateContextFastSearching;

namespace Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot
{
    public static class ImprovedBotBuilderExtensions
    {
        /// <summary>
        /// Note: Current middleware is registered automatically.
        /// <para></para>
        /// Invoked automatically in BotHandler.
        /// </summary>
        internal static void UseBotExt(this IPipelineBuilder @this)
        {
            @this.UseMiddlware<ImprovedBotMiddleware>();
        }

        /// <summary>
        /// Note: Current middleware is registered automatically.
        /// </summary>
        internal static void AddBotExt(this ServiceCollectionWrapper @this)
        {
            @this.Services.AddSingleton<ImprovedBotMiddleware>();
            @this.Services.AddSingleton<IUpdateContextSearchBag, UpdateContextSearchBag>();
            @this.Services.AddSingleton<IBotExtSingleton, BotExtSingleton>();
        }

        /// <summary>
        /// Note: Current middleware is registered automatically.
        /// <para></para>
        /// Invoked automatically in BotHandler.
        /// </summary>
        public static void AddBotExtGlobalValidator(this IPipelineBuilder @this, UpdateValidator updateValidator)
        {
            var botExtSingletone=@this.ServiceProvider.GetRequiredService<IBotExtSingleton>();
            botExtSingletone.GlobalValidators.Add(updateValidator);
        }
    }
}
