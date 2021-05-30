using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Exceptions;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot;

namespace Telegram.Bot.AspNetPipeline.Extensions.KeyboardKit
{
    public static class KeyboardKitExtensions
    {
        public static ISimpleKeyboard SimpleKeyboard(this UpdateContext @this)
        {
            const string ctxPropName = "_SimpleKeyboard";
            ISimpleKeyboard simpleKeyboard = null;
            if (@this.Properties.TryGetValue(ctxPropName, out var val))
            {
                try
                {
                    simpleKeyboard = (ISimpleKeyboard)val;
                }
                catch (Exception ex)
                {
                    throw new InvalidCastException(
                        $"Can`t cast UpdateContext[\"{ctxPropName}\"] value '{val}' to type '{typeof(ISimpleKeyboard)}'.",
                        ex
                    );
                }
            }
            else
            {
                simpleKeyboard = new SimpleKeyboard(@this);
                @this.Properties[ctxPropName] = simpleKeyboard;
            }
            return simpleKeyboard;
        }

        public static IManagedKeyboard ManagedKeyboard(this UpdateContext @this)
        {
            const string ctxPropName = "_ManagedKeyboard";
            IManagedKeyboard managedKeyboard = null;
            if (@this.Properties.TryGetValue(ctxPropName, out var val))
            {
                try
                {
                    managedKeyboard = (IManagedKeyboard)val;
                }
                catch (Exception ex)
                {
                    throw new InvalidCastException(
                        $"Can`t cast UpdateContext[\"{ctxPropName}\"] value '{val}' to type '{typeof(ManagedKeyboard)}'.",
                        ex
                    );
                }
            }
            else
            {
                managedKeyboard = new ManagedKeyboard(@this);
                @this.Properties[ctxPropName] = managedKeyboard;
            }
            return managedKeyboard;
        }

        /// <summary>
        /// Used in telegram bot replyMarkup.
        /// </summary>
        public static IList<IList<T>> ConvertToMultiple<T>(this IEnumerable<T> botButtons, bool twoInRow)
        {
            var resList = new List<IList<T>> { };

            if (twoInRow)
            {
                var i = 0;
                var inputButtonsList = (botButtons as IList<T>) ?? botButtons.ToList();
                while (i < inputButtonsList.Count)
                {
                    if (i + 1 < inputButtonsList.Count)
                    {
                        var smallList = new List<T>
                        {
                            inputButtonsList[i],
                            inputButtonsList[++i]
                        };

                        resList.Add(smallList);
                    }
                    else
                    {
                        resList.Add(new List<T>
                        {
                            inputButtonsList[i]
                        });
                    }

                    i++;
                }
            }
            else
            {
                foreach (var obj in botButtons)
                {
                    resList.Add(new List<T> { obj });
                }
            }
            return resList;
        }
    }
}