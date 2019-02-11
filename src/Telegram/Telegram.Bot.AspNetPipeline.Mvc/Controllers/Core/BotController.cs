using System.Threading;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot;
using Telegram.Bot.AspNetPipeline.Mvc.Core;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core
{
    public abstract class BotController
    {
        public ControllerActionContext ControllerContext { get; private set; }

        #region Proxy properties.
        public UpdateContext UpdateContext => ControllerContext.UpdateContext;

        public Update Update => ControllerContext.UpdateContext.Update;

        public Message Message => Update.Message;

        public Chat Chat => Update.Message.Chat;

        public BotClientContext BotContext => UpdateContext.BotContext;

        public ITelegramBotClient Bot => UpdateContext.BotContext.Bot;

        /// <summary>
        /// Just proxy to UpdateContext.BotExt().
        /// </summary>
        public BotExt BotExt => UpdateContext.BotExt;

        public IMvcFeatures Features => ControllerContext.Features ;

        public CancellationToken UpdateProcessingAborted => UpdateContext.UpdateProcessingAborted;
        #endregion

        bool _isInit;

        #region Init region
        /// <summary>
        /// Called after controller created to set context.
        /// </summary>
        private void Initialize(ControllerActionContext controllerActionContext)
        {
            if (_isInit)
            {
                throw new System.Exception("You can`t init controller twice.");
            }
            ControllerContext = controllerActionContext;
            _isInit = true;
            AfterInitialized();
        }

        /// <summary>
        /// Invoked after context initialized.
        /// </summary>
        protected virtual void AfterInitialized()
        {
        }

        /// <summary>
        /// Used to invoke initializer after controller constructed but before controller routing method invoked.
        /// </summary>
        public static void InvokeInitializer(BotController controller, ControllerActionContext controllerActionContext)
        {
            controller.Initialize(controllerActionContext);
        }
        #endregion
    }
}
