namespace Telegram.Bot.AspNetPipeline.Mvc.Extensions.Main
{
    /// <summary>
    /// Implemented with <see cref="ServicesBus"/>.
    /// </summary>
    public interface IOuterMiddlewaresInformerProvider
    {
        IOuterMiddlewaresInformer OuterMiddlewaresInformer { get; }
    }
}
