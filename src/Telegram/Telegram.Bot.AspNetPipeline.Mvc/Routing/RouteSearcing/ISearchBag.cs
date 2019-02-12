using System.Collections.Generic;
using Telegram.Bot.AspNetPipeline.Mvc.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing
{
    public interface ISearchBag
    {
        /// <summary>
        /// Return all found data for current search bag.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ActionDescriptor> GetFoundData();
    }

    public interface ISearchBag<TInnerBag> : ISearchBag
        where TInnerBag : class
    {
        IEnumerable<TInnerBag> All();
    }
}
