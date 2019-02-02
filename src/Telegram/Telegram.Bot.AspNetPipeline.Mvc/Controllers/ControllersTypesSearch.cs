using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers
{
    public static class ControllersTypesSearch
    {
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
