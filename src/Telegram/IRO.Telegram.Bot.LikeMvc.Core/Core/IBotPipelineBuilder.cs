using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace IRO.Telegram.Bot.ProcessingPipeline.Core
{
    /// <summary>
    /// Like IApplicationBuilder in asp.net .
    /// </summary>
    public interface IBotPipelineBuilder
    {
        /// <summary>
        /// Use it to configure your services.
        /// </summary>
        IServiceCollection Services { get; }

        void Use(MiddlewareDelegate middlewareDelegate);

        /// <summary>
        /// ASP.NET has one type of middleware (based on HttpContext).
        /// <para></para>
        /// But in current lib was added two types. Current type called before all default middlewares
        /// and before UpdateContext created.
        /// </summary>
        void UseInitial(InitialMiddlewareDelegate initialMiddlewareDelegate);

        InitialMiddlewareDelegate Build();
    }
}
