using System;
using System.Collections.Generic;
using System.Linq;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing.Implementions
{
    internal class GlobalSearchBag : IGlobalSearchBag
    {
        readonly IReadOnlyDictionary<int, IOrderScopeSearchBag> _orderScopeSearchBags;

        readonly IEnumerable<RouteDescriptionData> _routeDescriptions;

        readonly IReadOnlyDictionary<string, RouteDescriptionData> _routeDescriptionsByName;

        readonly IOrderedEnumerable<IOrderScopeSearchBag> _allSorted;

        public GlobalSearchBag(IEnumerable<RouteDescriptionData> routeDescriptions)
        {
            if(routeDescriptions==null)
                throw new ArgumentNullException(nameof(routeDescriptions));
            _routeDescriptions=routeDescriptions
                .Where(x => x.RouteInfo != null)
                .ToList();
            _routeDescriptionsByName = _routeDescriptions
                .Where(x => x.RouteInfo?.Name != null)
                .ToDictionary(x => x.RouteInfo.Name);

            _orderScopeSearchBags = PrepareDict(_routeDescriptions);
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
        public IEnumerable<RouteDescriptionData> GetFoundData()
        {
            return _routeDescriptions;
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

        public RouteDescriptionData FindByName(string routeActionName)
        {
            if (_routeDescriptionsByName.TryGetValue(routeActionName, out var value))
            {
                return value;
            }
            return default(RouteDescriptionData);
        }

        /// <summary>
        /// Return enumerable sorted by order number.
        /// </summary>
        public IOrderedEnumerable<IOrderScopeSearchBag> AllSorted()
        {
            return _allSorted;
        }

        IReadOnlyDictionary<int, IOrderScopeSearchBag> PrepareDict(IEnumerable<RouteDescriptionData> routeDescriptions)
        {
            var dict = new Dictionary<int, List<RouteDescriptionData>>();
            foreach (var routDesc in routeDescriptions)
            {
                var order = routDesc.RouteInfo.Order;
                if (!dict.TryGetValue(order, out var list))
                {
                    dict[order] = list = new List<RouteDescriptionData>();
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
