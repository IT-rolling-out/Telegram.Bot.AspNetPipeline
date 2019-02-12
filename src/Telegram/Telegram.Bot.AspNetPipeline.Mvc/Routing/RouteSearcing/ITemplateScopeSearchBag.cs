using System.Collections.Generic;
using Telegram.Bot.AspNetPipeline.Mvc.Core;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing
{
    public interface ITemplateScopeSearchBag : ISearchBag
    {
        string Template { get; }

        /// <summary>
        /// Note that we found many data, because one UpdateType can be used
        /// with different templates.
        /// <para></para>
        /// Return default of type if not found.
        /// </summary>
        IEnumerable<ActionDescriptor> FindByUpdateTypes(IEnumerable<UpdateType> updateTypes, int limit = int.MaxValue);
    }
}
