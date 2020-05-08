using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot.UpdateContextFastSearching;

//ReSharper disable CheckNamespace
namespace Telegram.Bot.AspNetPipeline.Extensions
{
    public static class ImprovedBotBuilderExtensions
    {
        /// <summary>
        /// Note: Current middleware is registered automatically.
        /// </summary>
        internal static void UseBotExt(this IPipelineBuilder @this)
        {
            @this.UseMiddleware<ImprovedBotMiddleware>();
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
        /// </summary>
        public static void AddBotExtGlobalValidator(this IPipelineBuilder @this, UpdateValidatorDelegate updateValidator)
        {
            var botExtSingletone=@this.ServiceProvider.GetRequiredService<IBotExtSingleton>();
            botExtSingletone.GlobalValidators.Add(updateValidator);
        }
    }
}
