using System;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Mvc.Routing;

namespace Telegram.Bot.AspNetPipeline.Mvc.Extensions
{
    public class OuterMiddlewaresService:IOuterMiddlewaresService
    {
        /// <summary>
        /// If mvc has method with bigger priority - return true.
        /// </summary>
        public Task<bool> CheckMvcHasPriorityHandler(RouteInfo routeInfo)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// If mvc has method with bigger priority - return true.
        /// <para></para>
        /// Lower number mean bigger priority.
        /// </summary>
        public Task<bool> CheckMvcHasPriorityHandler(int yourMethodOrder)
        {
            throw new NotImplementedException();
        }
    }
}

