using System.Linq;
using Telegram.Bot.AspNetPipeline.Mvc.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing
{
    public interface IGlobalSearchBag:ISearchBag<IOrderScopeSearchBag>
    {
        /// <summary>
        /// Find order scope by order number.
        /// Always one order scope.
        /// <para></para>
        /// Return default of type if not found.
        /// </summary>
        IOrderScopeSearchBag FindOrderScope(int order);

        ActionDescriptor FindByName(string routeActionName);

        /// <summary>
        /// Return enumerable sorted by order number.
        /// </summary>
        IOrderedEnumerable<IOrderScopeSearchBag> AllSorted();
    }
}
