using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions.Session;

//ReSharper disable CheckNamespace
namespace Telegram.Bot.AspNetPipeline.Extensions
{
    public static class UpdateContextExtensions
    {
        /// <summary>
        /// Return <see cref="ISessionStorage"/> of current chat.
        /// </summary>
        public static ISessionStorage Session(this UpdateContext @this)
        {
            var sessionProvider=@this.Services.GetRequiredService<ISessionStorageProvider>();
            var chatId=@this.ChatId.Identifier;
            return sessionProvider.ResolveSessionStorage(chatId);
        } 
    }
}
