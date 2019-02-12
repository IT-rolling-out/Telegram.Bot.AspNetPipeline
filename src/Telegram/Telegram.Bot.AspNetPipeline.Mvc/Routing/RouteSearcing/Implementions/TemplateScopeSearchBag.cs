using System;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.AspNetPipeline.Mvc.Core;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing.Implementions
{
    internal class TemplateScopeSearchBag : ITemplateScopeSearchBag
    {
        readonly IEnumerable<ActionDescriptor> _routes;

        public string Template { get; }

        public TemplateScopeSearchBag(string template, IEnumerable<ActionDescriptor> routes)
        {
            Template = template ?? throw new ArgumentNullException(nameof(template));
            _routes = routes ?? throw new ArgumentNullException(nameof(routes));
        }

        /// <summary>
        /// Note that we found many RouteSearchData's, because one UpdateType can be used
        /// with different templates.
        /// <para></para>
        /// Return default of type if not found.
        /// </summary>
        public IEnumerable<ActionDescriptor> FindByUpdateTypes(
            IEnumerable<UpdateType> updateTypes,
            int limit = int.MaxValue
            )
        {
            var res = new List<ActionDescriptor>();
            foreach (var routeDesc in _routes)
            {
                bool updatesAllowed = true;
                foreach (var item in updateTypes)
                {
                    if (!routeDesc.RouteInfo.UpdateTypes.Contains(item))
                    {
                        updatesAllowed = false;
                        break;
                    }
                }

                if (!updatesAllowed)
                    continue;
                res.Add(routeDesc);
                if (res.Count >= limit)
                {
                    break;
                }
            }

            return res;
        }

        public IEnumerable<ActionDescriptor> GetFoundData()
        {
            return _routes;
        }
    }
}
