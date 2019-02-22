using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.AspNetPipeline.Mvc.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing.Implementions
{
    public class GlobalSearchBag : IGlobalSearchBag
    {
        readonly IReadOnlyDictionary<int, IOrderScopeSearchBag> _orderScopeSearchBags;

        readonly IEnumerable<ActionDescriptor> _routes;

        readonly IReadOnlyDictionary<string, ActionDescriptor> _routesByName;

        readonly IOrderedEnumerable<IOrderScopeSearchBag> _allSorted;

        public GlobalSearchBag(IEnumerable<ActionDescriptor> routeDescriptions)
        {
            if(routeDescriptions==null)
                throw new ArgumentNullException(nameof(routeDescriptions));
            _routes=routeDescriptions
                .Where(x => x.RouteInfo != null)
                .ToList();
            _routesByName = _routes
                .Where(x => x.RouteInfo?.Name != null)
                .ToDictionary(x => x.RouteInfo.Name);

            _orderScopeSearchBags = PrepareDict(_routes);
            _allSorted = _orderScopeSearchBags
                .Select(x => x.Value)
                .OrderBy((x) => x.Order);

        }

        public IEnumerable<IOrderScopeSearchBag> All()
        {
            return _allSorted;
        }

        /// <summary>
        /// Return all found SearchData for current search bag.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ActionDescriptor> GetFoundData()
        {
            return _routes;
        }

        /// <summary>
        /// Find order scope by order number.
        /// Always one order scope.
        /// <para></para>
        /// Return default of type if not found.
        /// </summary>
        public IOrderScopeSearchBag FindOrderScope(int order)
        {
            if (_orderScopeSearchBags.TryGetValue(order, out var value))
            {
                return value;
            }
            return null;
        }

        public ActionDescriptor FindByName(string routeActionName)
        {
            if (_routesByName.TryGetValue(routeActionName, out var value))
            {
                return value;
            }
            return default(ActionDescriptor);
        }

        /// <summary>
        /// Return enumerable sorted by order number.
        /// </summary>
        public IOrderedEnumerable<IOrderScopeSearchBag> AllSorted()
        {
            return _allSorted;
        }

        IReadOnlyDictionary<int, IOrderScopeSearchBag> PrepareDict(IEnumerable<ActionDescriptor> routeDescriptions)
        {
            var dict = new Dictionary<int, List<ActionDescriptor>>();
            foreach (var routDesc in routeDescriptions)
            {
                var order = routDesc.RouteInfo.Order;
                if (!dict.TryGetValue(order, out var list))
                {
                    dict[order] = list = new List<ActionDescriptor>();
                }
                list.Add(routDesc);
            }

            var res = new Dictionary<int, IOrderScopeSearchBag>();
            foreach (var item in dict)
            {
                var templateScope = new OrderScopeSearchBag(item.Key, item.Value);
                res.Add(item.Key, templateScope);
            }
            return res;
        }
    }
}
