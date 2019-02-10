using System;
using System.Collections.Generic;
using System.Linq;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing.Implementions
{
    internal class OrderScopeSearchBag : IOrderScopeSearchBag
    {
        readonly IReadOnlyDictionary<string, ITemplateScopeSearchBag> _templateScopeSearchBags;

        readonly IEnumerable<RouteDescriptionData> _routeDescriptions;

        readonly IEnumerable<ITemplateScopeSearchBag> _all;

        public int Order { get; }

        public OrderScopeSearchBag(int order, IEnumerable<RouteDescriptionData> routeDescriptions)
        {
            Order = order;
            _routeDescriptions = routeDescriptions ?? throw new ArgumentNullException(nameof(routeDescriptions));
            _templateScopeSearchBags = PrepareDict(_routeDescriptions);
            _all = _templateScopeSearchBags.Select(x => x.Value);
        }

        /// <summary>
        /// Find template scope. TemplateScopeSearchBag can be only one for string.
        /// If RouteInfo template contains null or whitespace string - template scope is "".
        /// <para></para>
        /// Null or whitespace template is equals to empty string.
        /// <para></para>
        /// Return default of type if not found.
        /// </summary>
        public ITemplateScopeSearchBag FindTemplateScope(string template)
        {
            template = RealTemplate(template);
            if (_templateScopeSearchBags.TryGetValue(template, out var value))
            {
                return value;
            }
            return null;
        }

        public IEnumerable<ITemplateScopeSearchBag> All()
        {
            return _all;
        }

        /// <summary>
        /// Return all found SearchData for current search bag.
        /// </summary>
        public IEnumerable<RouteDescriptionData> GetFoundData()
        {
            return _routeDescriptions;
        }

        IReadOnlyDictionary<string, ITemplateScopeSearchBag> PrepareDict(IEnumerable<RouteDescriptionData> routeDescriptions)
        {
            var dict = new Dictionary<string, List<RouteDescriptionData>>();
            foreach (var routDesc in routeDescriptions)
            {
                var template = RealTemplate(routDesc.RouteInfo.Template);
                if (!dict.TryGetValue(template, out var list))
                {
                    dict[template] = list = new List<RouteDescriptionData>();
                }
                list.Add(routDesc);
            }

            var res = new Dictionary<string, ITemplateScopeSearchBag>();
            foreach (var item in dict)
            {
                var templateScope = new TemplateScopeSearchBag(item.Key, item.Value);
                res.Add(item.Key, templateScope);
            }
            return res;
        }

        string RealTemplate(string template)
        {
            if (string.IsNullOrWhiteSpace(template))
                return "";
            return template;
        }
    }
}
