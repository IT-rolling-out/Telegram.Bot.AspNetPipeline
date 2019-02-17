using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing.Routers
{
    /// <summary>
    /// Used only to aggregate all routers.
    /// <para></para>
    /// Registered in IOC as self. Please, don't use it in your implementions.
    /// </summary>
    public class MainRouter:IRouter
    {
        readonly IEnumerable<IRouter> _routers;

        public MainRouter(IEnumerable<IRouter> routers)
        {
            if(routers==null)
                throw new ArgumentNullException(nameof(routers));
            _routers = routers.ToList();
        }

        public async Task RouteAsync(RoutingContext routeContext)
        {
            foreach (var router in _routers)
            {
                if (routeContext.ActionDescriptor?.Handler!=null)
                    return;
                await router.RouteAsync(routeContext);
            }
        }
    }
}
