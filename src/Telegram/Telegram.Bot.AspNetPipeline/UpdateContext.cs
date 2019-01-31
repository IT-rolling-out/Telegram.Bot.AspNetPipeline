using System;
using System.Collections.Generic;
using System.Threading;
using Telegram.Bot.AspNetPipeline.Core.ImprovedBot;
using Telegram.Bot.AspNetPipeline.Implementations;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Core
{
    /// <summary>
    /// Just like http context in asp.net mvc.
    /// </summary>
    public class UpdateContext: IDisposable
    {
        /// <summary>
        /// Unique id, used in GetHashCode too.
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();

        public Update Update { get; }

        public BotClientContext BotContext { get; }

        /// <summary>
        /// BotExtensions based on UpdateContext and cant work without it.
        /// Thats why it here, not in BotContext.
        /// </summary>
        public BotExt BotExt { get; }

        /// <summary>
        /// Scoped services for current update.
        /// </summary>
        public IServiceProvider Services { get; }

        #region Proxy properties.
        public ITelegramBotClient Bot => BotContext.Bot;

        public Message Message => Update.Message;

        public Chat Chat => Update.Message.Chat;
        #endregion

        #region Properties bag.
        IDictionary<object, object> _properties;

        /// <summary>
        /// Stores arbitrary metadata properties associated with current update request.
        /// </summary>
        public IDictionary<object, object> Properties
        {
            get
            {
                if (_properties == null)
                {
                    _properties = new Dictionary<object, object>();
                }
                return _properties;
            }
        }
        #endregion

        /// <summary>
        /// Just like RequestAborted in HttpContex. 
        /// Used in BotExtensions.ReadMessage and etc.
        /// </summary>
        public CancellationToken UpdateProcessingAborted { get; }

        

        public UpdateContext(
            Update update,
            BotClientContext botContext,
            IServiceProvider services,
            CancellationToken updateProcessingAborted,
            IBotExtSingleton botExtSingleton
            )
        {
            Update = update;
            BotContext = botContext;
            Services = services;
            UpdateProcessingAborted = updateProcessingAborted;
            BotExt =new BotExt(botExtSingleton, this);
        }

        #region Processing status.
        /// <summary>
        /// If you set it to true (call Processed method) next middlewares will know that Update was processed.
        /// They can ignore it or finish their work.
        /// Mvc middleware will set it automatically for all controller actions and read-callbacks.
        /// </summary>
        public bool IsProcessed { get; private set; }

        public void Processed()
        {
            IsProcessed = true;

        }

        public bool ForceExitRequested { get; private set; }

        /// <summary>
        /// Set IsProcessed and ForceExitRequested to true.
        /// Next middleware action will not be executed.
        /// </summary>
        public void ForceExit()
        {
            Processed();
            ForceExitRequested = true;
        }

        #endregion

        public override int GetHashCode()
        {
            return Id.GetHashCode() + 279;
        }

        #region Dispose region
        public bool IsDisposed { get; private set; }

        public event Func<UpdateContext> Disposed;

        public void Dispose()
        {
            if (IsDisposed)
                return;
            try
            {
                var hiddenContext=(HiddenUpdateContext)Properties[HiddenUpdateContext.DictKeyName] ;
                hiddenContext.UpdateProcessingAbortedSource.Cancel();
            }
            catch { }
            Properties.Clear();
            IsDisposed = true;
            Disposed?.Invoke();
        }
        #endregion
    }
}
