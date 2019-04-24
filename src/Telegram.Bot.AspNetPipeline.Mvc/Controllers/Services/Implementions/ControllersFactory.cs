using System;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.Services.Implementions
{
    public class ControllersFactory:IControllersFactory
    {
        /// <summary>
        /// Use only IOC container to resolve controllers.
        /// </summary>
        public BotController Create(ControllerActionContext controllerActionContext)
        {
            var serv=controllerActionContext.UpdateContext.Services;
            var type=controllerActionContext.ActionDescriptor.ControllerType;
            var controller=(BotController)serv.GetService(type);
            return controller;
        }
    }
}
