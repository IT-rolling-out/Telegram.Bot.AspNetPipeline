using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Core.Builder;

namespace Telegram.Bot.AspNetPipeline.Implementations
{
    public class PipelineBuilder:IPipelineBuilder
    {
        Stack<UpdateProcessingDelegate> _updateProcessingDelegates=new Stack<UpdateProcessingDelegate>();

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
            UpdateProcessingDelegate processingDelegate_OfIteration =async (UpdateContext ctx, Func<Task> nextFunc) => { };
            while (_updateProcessingDelegates.Count > 0)
            {
                var prevProcessingDelegate = processingDelegate_OfIteration;
                var currentProcessingDelegate = _updateProcessingDelegates.Pop();

                processingDelegate_OfIteration= async (UpdateContext ctx, Func<Task> mostNext) =>
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

            return processingDelegate_OfIteration;
        }
    }
}
