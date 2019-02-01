using System;
using System.Collections.Generic;

namespace Telegram.Bot.AspNetPipeline.Mvc.Builder
{
    public interface IMvcServicesRegistrator
    {
        /// <summary>
        /// Use it to add your controllers.
        /// </summary>
        void ConfigureControllers(
            Action<IList<Type>> editControllersListDelegate,
            bool findWithReflection = true
            );
    }
}
