﻿using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.MiddlewareServices
{
    public class ControllersFactory:IControllersFactory
    {
        public BotController Create(ControllerActionDescriptor controllerActionDescriptor)
        {
            throw new System.NotImplementedException();
        }
    }
}
