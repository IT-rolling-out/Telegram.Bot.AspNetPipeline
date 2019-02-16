using System;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Extensions.Main
{
    /// <summary>
    /// Now just can inform middleware to execute action, not do it self.
    /// </summary>
    public class MvcFeatures : IMvcFeatures
    {
        const string StartAnotherActionDataName = "StartAnotherActionData";

        public MvcFeatures()
        {
            //?Injected through IMvcFeaturesProvider, but now it has no reason to use it. So, why?
            //At start was planned some features, that interact with MvcMiddleware, so i init current service in ServiceBus.
            //Those features can be added in feature, so i left it as is.
        }

        /// <summary>
        /// Started another controller action when current action execution finished.
        /// Just pass current <see cref="UpdateContext"/> (not <see cref="ActionContext"/>, it will be created for new action) to another action.
        /// <para></para>
        /// Will not be executed if <see cref="UpdateContext.ForceExit"/> was invoked.
        /// </summary>
        /// <param name="name">Name property from BotRouteAttribute.</param>
        public void StartAnotherAction(ActionContext actionContext, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Can't be null or white space.", nameof(name));
            actionContext.Properties[StartAnotherActionDataName] = new StartAnotherActionData()
            {
                ActionName = name
            };
        }

        /// <summary>
        /// Return data or null.
        /// </summary>
        public static StartAnotherActionData GetData(ActionContext actionContext)
        {
            if (actionContext.Properties.TryGetValue(StartAnotherActionDataName, out var val))
            {
                return (StartAnotherActionData)val;
            }
            return null;
        }
    }
}
