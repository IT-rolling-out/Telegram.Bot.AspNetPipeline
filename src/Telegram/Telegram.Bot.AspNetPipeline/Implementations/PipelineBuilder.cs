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
            _updateProcessingDelegates.Push(middlewareDelegate);
        }

        public UpdateProcessingDelegate Build()
        {
            UpdateProcessingDelegate res = async (UpdateContext ctx, Func<Task> nextFunc) =>
            {
                //"next" delegate for current iteration. Started with empty delegate for last middleware.
                //For each new iteration and each middleware from stack nextOfIteration will contain delegate to execute all next middleware.
                Func<Task> nextOfIteration = async () => { };
                while (_updateProcessingDelegates.Count > 0)
                {
                    //Get last delegate.
                    var updateProcessingDelegate = _updateProcessingDelegates.Pop();

                    //Local next is unique for each updateProcessingDelegate.
                    var next = nextOfIteration;
                    //Creating delegate with "saved" next delegate.
                    nextOfIteration = ()=> updateProcessingDelegate?.Invoke(ctx, next);
                }
            };
            return res;
        }
    }
}
