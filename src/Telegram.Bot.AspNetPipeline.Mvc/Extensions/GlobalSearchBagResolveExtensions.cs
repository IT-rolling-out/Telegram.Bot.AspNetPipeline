using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing;

namespace Telegram.Bot.AspNetPipeline.Mvc.Extensions
{
    public static class GlobalSearchBagResolveExtensions
    {
        internal static void SetGlobalSearchBag(this UpdateContext ctx, IGlobalSearchBag globalSearchBag)
        {
            ctx.Properties["_GlobalSearchBag"] = globalSearchBag;
        }

        /// <summary>
        /// Works only with MVC.
        /// </summary>
        internal static IGlobalSearchBag GlobalSearchBag(this UpdateContext ctx)
        {
            if (ctx.Properties.TryGetValue("_GlobalSearchBag", out var globalSearchBag))
            {
                return (IGlobalSearchBag)globalSearchBag;
            }
            else
            {
                throw new Exception("Can't find IGlobalSearchBag, maybe mvc not used.");
            }
        }
    }
}
