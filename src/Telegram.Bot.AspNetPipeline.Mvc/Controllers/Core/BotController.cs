using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot;
using Telegram.Bot.AspNetPipeline.Extensions.KeyboardKit;
using Telegram.Bot.AspNetPipeline.Extensions.Session;
using Telegram.Bot.AspNetPipeline.Mvc.Extensions;
using Telegram.Bot.AspNetPipeline.Mvc.Extensions.MvcFeatures;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core
{
    public abstract class BotController
    {
        public ControllerActionContext ControllerContext { get; private set; }

        #region Proxy properties.
        public UpdateContext UpdateContext => ControllerContext.UpdateContext;

        public Update Update => ControllerContext.UpdateContext.Update;

        public Message Message => UpdateContext.Message;

        /// <summary>
        /// Return chat id for any update type. If chat can't be extracted from current update - return user id.
        /// </summary>
        public ChatId ChatId => Update.ExtractChatId();

        /// <summary>
        /// Return from id for any update type.
        /// </summary>
        public ChatId FromId => Update.ExtractFromId();

        public Chat Chat => UpdateContext.Chat;

        public BotClientContext BotContext => UpdateContext.BotContext;

        public ITelegramBotClient Bot => BotContext.Bot;

        /// <summary>
        /// Just proxy to UpdateContext.BotExt().
        /// </summary>
        public BotExt BotExt => UpdateContext.BotExt;

        public CancellationToken UpdateProcessingAborted => UpdateContext.UpdateProcessingAborted;

        public ISimpleKeyboard SimpleKeyboard => UpdateContext.SimpleKeyboard();

        /// <summary>
        /// Work only in current request. 
        /// </summary>
        public IManagedKeyboard ManagedKeyboard => UpdateContext.ManagedKeyboard();

        /// <summary>
        /// Just proxy to ControllerContext.Features().
        /// </summary>
        public ContextMvcFeatures Features => ControllerContext.Features();

        /// <summary>
        /// Just proxy to UpdateContext.Logger().
        /// Useful for fast easy logging, but better to create logger by <see cref="ILoggerFactory"/>.
        /// </summary>
        public ILogger Logger => UpdateContext.Logger();

        public bool IsModelStateValid => ControllerContext.IsModelStateValid;

        /// <summary>
        /// Session of current chat. Include it's namespace to use extensions.
        /// </summary>
        public ISessionStorage Session => UpdateContext.Session();

        /// <summary>
        /// Send text message to current chat.
        /// <para>Proxy to <see cref="UpdateContext"/> extension method.</para>
        /// </summary>
        public async Task<Message> SendTextMessageAsync(
            string text,
            ParseMode parseMode = ParseMode.Default,
            bool disableWebPagePreview = false,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = default(CancellationToken)
            )
        {
            return await UpdateContext.SendTextMessageAsync(
                text,
                parseMode,
                disableWebPagePreview,
                disableNotification,
                replyToMessageId,
                replyMarkup,
                cancellationToken
                );
        }
        #endregion

        bool _isInit;

        #region Init region
        /// <summary>
        /// Called after controller created to set context.
        /// </summary>
        private async Task Initialize(ControllerActionContext controllerActionContext)
        {
            if (_isInit)
            {
                throw new System.Exception("You can`t init controller twice.");
            }
            ControllerContext = controllerActionContext;
            _isInit = true;
            await Initialized();
        }

        /// <summary>
        /// Invoked after context initialized.
        /// </summary>
        protected virtual async Task Initialized()
        {
        }

        /// <summary>
        /// Invoked after every update processing method invoked.
        /// Will not be invoked on exceptions.
        /// Use it to finish all work in controller. 
        /// </summary>
        protected virtual async Task Processed()
        {
        }

        /// <summary>
        /// Used to invoke initializer after controller constructed but before controller routing method invoked.
        /// </summary>
        internal static async Task InvokeInitialize(BotController controller, ControllerActionContext controllerActionContext)
        {
            await controller.Initialize(controllerActionContext);
        }

        /// <summary>
        /// Used to invoke <see cref="Processed"/>.
        /// </summary>
        internal static async Task InvokeProcessed(BotController controller)
        {
            await controller.Processed();
        }
        #endregion
    }
}
