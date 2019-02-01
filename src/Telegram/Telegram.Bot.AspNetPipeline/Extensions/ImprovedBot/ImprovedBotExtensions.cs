using System;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Core;

namespace Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot
{
    public static class ImprovedBotExtensions
    {
        /// <summary>
        /// BotExtensions based on UpdateContext and cant work without it.
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
                    botExt = new BotExt(@this.Services.GetService<IBotExtSingleton>(), @this);
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
                throw new Exception(
                    "BotExt resolve exception. Maybe middleware ImprovedBot wasn`t registered.", 
                    ex
                    );
            }

            
        }
    }
}
