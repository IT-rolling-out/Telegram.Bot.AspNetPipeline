using System;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Exceptions;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot;

//ReSharper disable CheckNamespace
namespace Telegram.Bot.AspNetPipeline.Extensions
{
    public static class ImprovedBotExtensions
    {
        /// <summary>
        /// BotExtensions based on UpdateContext and can't work without it.
        /// Thats why it here, not in BotContext.
        /// </summary>
        public static BotExt BotExt(this UpdateContext @this)
        {
            const string ctxPropName = "_BotExt";
            try
            {
                BotExt botExt = null;
                if (@this.Properties.TryGetValue(ctxPropName, out var val))
                {
                    try
                    {
                        botExt = (BotExt)val;
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidCastException(
                            $"Can`t cast UpdateContext[\"{ctxPropName}\"] value '{val}' to type '{typeof(BotExt)}'.",
                            ex
                        );
                    }
                }
                else
                {
                    botExt = new BotExt(@this.Services.GetRequiredService<IBotExtSingleton>(), @this);
                    @this.Properties[ctxPropName] = botExt;
                }

                if (botExt == null)
                {
                    throw new NullReferenceException("Found BotExt value is null.");
                }
                return botExt;
            }
            catch (Exception ex)
            {
                throw new TelegramAspException(
                    "BotExt resolve exception. Maybe middleware ImprovedBot wasn`t registered.", 
                    ex
                    );
            }

            
        }
    }
}
