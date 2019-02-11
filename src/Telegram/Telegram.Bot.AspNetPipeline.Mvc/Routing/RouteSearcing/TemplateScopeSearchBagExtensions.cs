using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.AspNetPipeline.Mvc.Core;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing
{
    public static class TemplateScopeSearchBagExtensions
    {
        public static RouteDescriptionData FindOneByUpdateTypes(this ITemplateScopeSearchBag @this, IEnumerable<UpdateType> updateTypes)
        {
            return @this.FindByUpdateTypes(updateTypes, 1).FirstOrDefault();
        }

        public static RouteDescriptionData FindOneByUpdateType(this ITemplateScopeSearchBag @this, UpdateType updateType)
        {
            return @this.FindByUpdateType(updateType, 1).FirstOrDefault();
        }

        public static IEnumerable<RouteDescriptionData> FindByUpdateType(
            this ITemplateScopeSearchBag @this,
            UpdateType updateType,
            int limit = int.MaxValue
            )
        {
            return @this.FindByUpdateTypes(
                new UpdateType[] { updateType },
                limit
                );
        }
    }

}
