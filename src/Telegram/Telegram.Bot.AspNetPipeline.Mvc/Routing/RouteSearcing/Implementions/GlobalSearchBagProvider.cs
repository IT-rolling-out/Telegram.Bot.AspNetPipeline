using System;
using System.Collections.Generic;
using Telegram.Bot.AspNetPipeline.Exceptions;
using Telegram.Bot.AspNetPipeline.Mvc.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing.Implementions
{
    public class GlobalSearchBagProvider : IGlobalSearchBagProvider
    {
        bool _isInit;

        GlobalSearchBag _globalSearchBagSingleton;

        public void Init(IEnumerable<ActionDescriptor> routeDescriptions)
        {
            if (_isInit)
                throw new TelegramAspException("Was init before.");
            routeDescriptions = routeDescriptions ?? throw new ArgumentNullException(nameof(routeDescriptions));
            _globalSearchBagSingleton = new GlobalSearchBag(routeDescriptions);
            _isInit = true;
        }

        public IGlobalSearchBag Resolve()
        {
            return _globalSearchBagSingleton;
        }
    }
}
