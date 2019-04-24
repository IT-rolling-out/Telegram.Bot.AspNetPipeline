using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Core;

namespace Telegram.Bot.AspNetPipeline.Builder
{
    internal class PipelineBuilder : IPipelineBuilder
    {
        readonly Stack<UpdateProcessingDelegate> _updateProcessingDelegates = new Stack<UpdateProcessingDelegate>();

        public IServiceProvider ServiceProvider { get; }

        public PipelineBuilder(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public void Use(UpdateProcessingDelegate middlewareDelegate)
        {
            if (middlewareDelegate == null)
            {
                throw new ArgumentNullException(nameof(middlewareDelegate));
            }
            _updateProcessingDelegates.Push(middlewareDelegate);
        }

        public UpdateProcessingDelegate Build()
        {
            //For each new iteration and each middleware from stack processingDelegate_OfIteration will contain delegate to execute
            //all next middleware.
            //"next" delegate for current iteration. Started with empty delegate for last middleware.
            UpdateProcessingDelegate processingDelegate_OfIteration = async (UpdateContext ctx, Func<Task> nextFunc) => { };
            while (_updateProcessingDelegates.Count > 0)
            {
                var prevProcessingDelegate = processingDelegate_OfIteration;
                var currentProcessingDelegate = _updateProcessingDelegates.Pop();

                processingDelegate_OfIteration = async (UpdateContext ctx, Func<Task> mostNext) =>
                 {
                     Func<Task> next = async () =>
                     {
                         await prevProcessingDelegate.Invoke(ctx, mostNext);
                     };

                     //Force exit.
                     if (ctx.ForceExitRequested)
                     {
                         return;
                     }

                     //Execute current.
                     await currentProcessingDelegate.Invoke(ctx, next);
                 };
            }

            //Just add some checks.
            UpdateProcessingDelegate res = async (UpdateContext ctx, Func<Task> next) =>
            {
                if (ctx == null)
                    throw new ArgumentNullException(nameof(ctx), "Null context passed to UpdateProcessingDelegate.");
                if (next == null)
                    throw new ArgumentNullException(nameof(next), "Null 'next' passed to UpdateProcessingDelegate.");
                await processingDelegate_OfIteration.Invoke(ctx, next);
            };
            return res;
        }
    }
}
