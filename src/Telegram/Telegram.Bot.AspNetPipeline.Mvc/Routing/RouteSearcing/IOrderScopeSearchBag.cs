namespace Telegram.Bot.AspNetPipeline.Mvc.Routing.RouteSearcing
{
    public interface IOrderScopeSearchBag:ISearchBag<ITemplateScopeSearchBag>
    {
        int Order { get; }

        /// <summary>
        /// Find template scope. TemplateScopeSearchBag can be only one for string.
        /// If RouteInfo template contains null or whitespace string - template scope is "".
        /// <para></para>
        /// Null or whitespace template is equals to empty string.
        /// <para></para>
        /// Return default of type if not found.
        /// </summary>
        ITemplateScopeSearchBag FindTemplateScope(string template);
    }
}
