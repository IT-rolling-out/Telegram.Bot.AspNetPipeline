using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.Services
{
    public static class ControllersTypesSearch
    {
        /// <summary>
        /// Find controllers with reflection.
        /// </summary>
        public static IList<Type> FindAllControllers()
        {
            var baseType = typeof(BotController);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies.SelectMany(s => s.GetTypes())
                .Where(t => baseType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsGenericTypeDefinition && t.IsClass)
                .ToList();
            return types;
        }
    }
}
