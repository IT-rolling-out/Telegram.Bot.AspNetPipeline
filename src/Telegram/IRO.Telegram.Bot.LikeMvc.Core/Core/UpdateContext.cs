using System;
using System.Collections.Generic;
using System.Threading;
using IRO.Telegram.Bot.ProcessingPipeline.Core.BotExt;
using Telegram.Bot.Types;
using Tg = Telegram.Bot;

namespace IRO.Telegram.Bot.ProcessingPipeline.Core
{
    /// <summary>
    /// Just like http context in asp.net mvc.
    /// </summary>
    public class UpdateContext
    {
        public Update Update { get; }

        public BotClientContext BotContext { get; }

        /// <summary>
        /// BotExtensions based on UpdateContext and cant work without it.
        /// Thats why it here, not in BotContext.
        /// </summary>
        public BotExtensions BotExtensions { get; }

        /// <summary>
        /// Scoped services for current update.
        /// </summary>
        public IServiceProvider Services { get; }

        #region Proxy properties.
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
            IBotStatelessExtensions botStatelessExtensions
            )
        {
            Update = update;
            BotContext = botContext;
            Services = services;
            UpdateProcessingAborted = updateProcessingAborted;
            BotExtensions = new BotExtensions(botStatelessExtensions, this);
        }
    }
}
