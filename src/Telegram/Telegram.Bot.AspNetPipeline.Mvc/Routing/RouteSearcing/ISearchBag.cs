using System.Collections.Generic;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing
{
    public interface ISearchBag
    {
        /// <summary>
        /// Return all found SearchData for current search bag.
        /// </summary>
        /// <returns></returns>
        IEnumerable<RouteDescriptionData> GetFoundData();
    }

    public interface ISearchBag<TInnerBag> : ISearchBag
        where TInnerBag : class
    {
        IEnumerable<TInnerBag> All();
    }
}
