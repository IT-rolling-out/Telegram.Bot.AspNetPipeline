using System;
using System.Collections.Generic;
using System.Linq;
using IRO.Common.Collections;
using Telegram.Bot.AspNetPipeline.Exceptions;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Mvc.Core;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing.Implementions
{
    public class GlobalSearchBag : IGlobalSearchBag
    {
        readonly IReadOnlyDictionary<int, IOrderScopeSearchBag> _orderScopeSearchBags;

        readonly IEnumerable<ActionDescriptor> _routes;

        readonly IReadOnlyDictionary<string, ActionDescriptor> _routesByName;

        readonly IOrderedEnumerable<IOrderScopeSearchBag> _allSorted;

        public GlobalSearchBag(IEnumerable<ActionDescriptor> routeDescriptions, bool checkEqualsRouteInfo)
        {
            if (routeDescriptions == null)
                throw new ArgumentNullException(nameof(routeDescriptions));
            _routes = routeDescriptions
                .Where(x => x.RouteInfo != null)
                .ToList();

            if (checkEqualsRouteInfo)
            {
                var routeInfoCollection = _routes.Select(r => r.RouteInfo).ToList();
                foreach (var routeInfo in routeInfoCollection)
                {
                    if (routeInfo == null)
                        continue;
                    if (ContainsEqualRouteInfo(routeInfoCollection, routeInfo, true))
                    {
                        throw new TelegramAspException(
                            $"Found completely identical RouteInfo '{routeInfo}'.\n" +
                            $"Set Name property or disable RouteInfo check (in MvcOptions) " +
                            $"if you did it intentionally."
                        );
                    }
                }
            }

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

        bool ContainsEqualRouteInfo(ICollection<RouteInfo> routeInfoCollection, RouteInfo routeInfo, bool ignoreSelf)
        {
            foreach (var item in routeInfoCollection)
            {
                bool valuesEqual = item.Name == routeInfo.Name && item.Order == routeInfo.Order && item.Template == routeInfo.Template;
                bool updateTypesEqual = UpdateTypeCollectionsEqual(item.UpdateTypes, routeInfo.UpdateTypes);
                bool isEqual = valuesEqual && updateTypesEqual;
                if (isEqual)
                {
                    bool isSelf = item == routeInfo;
                    if (!(ignoreSelf && isSelf))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #region UpdateType equals region.
        HashSet<UpdateType> _allUpdateTypesHashSet;

        bool UpdateTypeCollectionsEqual(ICollection<UpdateType> colection1, ICollection<UpdateType> colection2)
        {
            if (colection1 == null && colection2 == null)
                return true;

            if (_allUpdateTypesHashSet == null)
                _allUpdateTypesHashSet = EnumerableExtensions.ToHashSet(UpdateTypeExtensions.All);
            //Set all, because null equals to all templates allowed.
            colection1 = colection1 ?? _allUpdateTypesHashSet;
            colection2 = colection2 ?? _allUpdateTypesHashSet;

            foreach (var updateType in colection1)
            {
                if (!colection2.Contains(updateType))
                    return false;
            }
            foreach (var updateType in colection2)
            {
                if (!colection1.Contains(updateType))
                    return false;
            }
            return true;
        }
        #endregion
    }
}
