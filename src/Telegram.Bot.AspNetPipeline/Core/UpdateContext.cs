using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using IRO.Common.Abstractions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot;
using Telegram.Bot.Types;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Core.Internal;
using Telegram.Bot.AspNetPipeline.Core.Services;
using Telegram.Bot.AspNetPipeline.Extensions;

namespace Telegram.Bot.AspNetPipeline.Core
{
    /// <summary>
    /// Just like http context in ASP.NET MVC.
    /// </summary>
    [DataContract]
    public class UpdateContext : IInformativeDisposable
    {

        #region Chatting with bot
        bool _chattingWithBotValueCalculated;
        bool _chattingWithBot;

        /// <summary>
        /// Return true if current chat is chat with bot.
        /// <para></para>
        /// If chatId can't be resolved - return null.
        /// </summary>
        public bool? IsChattingWithBot
        {
            get
            {
                if (!_chattingWithBotValueCalculated)
                {
                    if (Update.Message != null)
                    {
                        _chattingWithBot = Message.Chat.Id == Message.From.Id;
                    }
                    _chattingWithBotValueCalculated = true;
                }
                return _chattingWithBot;
            }
        }
        #endregion

        /// <summary>
        /// Unique id, used in GetHashCode too.
        /// </summary>
        [DataMember]
        public Guid Id { get; } = Guid.NewGuid();

        [DataMember]
        public Update Update { get; }

        public BotClientContext BotContext { get; }

        /// <summary>
        /// Scoped services for current update.
        /// </summary>
        public IServiceProvider Services { get; }

        #region Proxy properties.
        public ITelegramBotClient Bot => BotContext.Bot;

        public Message Message => Update.Message;

        /// <summary>
        /// Null if message is null.
        /// </summary>
        public Chat Chat => Message?.Chat;
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

        public BotExt BotExt => ImprovedBotExtensions.BotExt(this);

        public UpdateContext(
            Update update,
            BotClientContext botContext,
            IServiceProvider services,
            CancellationToken updateProcessingAborted
            )
        {
            Update = update;
            BotContext = botContext;
            Services = services;
            UpdateProcessingAborted = updateProcessingAborted;
        }

        #region Processing status.
        /// <summary>
        /// If you set it to true (call Processed method) next middlewares will know that Update was processed.
        /// They can ignore it or finish their work.
        /// Mvc middleware will set it automatically for all controller actions and read-callbacks.
        /// </summary>
        [DataMember]
        public bool IsProcessed { get; private set; }

        public void Processed()
        {
            IsProcessed = true;
            this.Logger().LogTrace("Processed.");
        }

        [DataMember]
        public bool ForceExitRequested { get; private set; }

        /// <summary>
        /// Set IsProcessed and ForceExitRequested to true.
        /// Next middleware action will not be executed.
        /// <para></para>
        /// This method guarantees you that next() delegate will not be called,
        /// but more clear way is to handle it only by ignoring or invoking next() delegate.
        /// </summary>
        public void ForceExit()
        {
            Processed();
            ForceExitRequested = true;
            this.Logger().LogTrace("Force exit requested.");
        }

        #endregion

        public override int GetHashCode()
        {
            return Id.GetHashCode() + 279;
        }

        #region Dispose region.
        [DataMember]
        public bool IsDisposed { get; private set; }

        public event Action<UpdateContext> Disposed;

        public event Action<UpdateContext> Disposing;

        /// <summary>
        /// Don't use it in controllers to abort processing. Use <see cref="ForceExit"/>.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
                return;

            Disposing?.Invoke(this);
            try
            {
                var hiddenContext = (HiddenUpdateContext)Properties[HiddenUpdateContext.DictKeyName];
                hiddenContext.UpdateProcessingAbortedSource.Cancel();
            }
            catch { }

            ForceExit();
            this.Logger().LogTrace("Disposed.");
            Properties.Clear();
            IsDisposed = true;
            Disposed?.Invoke(this);
        }
        #endregion

        public override string ToString()
        {
            var baseName = base.ToString();
            if (Update.Message == null)
            {
                return $"{baseName}(ChatId={Chat?.Id}, Update(Id={Update.Id}, Type={Update.Type}))";
            }
            else
            {
                string msgText = Message.Text ?? "";
                const int wordsLimit = 20;
                if (msgText.Length > wordsLimit)
                {
                    msgText = msgText.Remove(wordsLimit - 3) + "...";
                }
                //Just use to encode.
                msgText = JsonConvert.SerializeObject(msgText);
                return $"{baseName}(ChatId={Chat?.Id}, Update(Id={Update.Id}, Type={Update.Type})," +
                       $" Message=(Id={Message.MessageId}, FromId={Message.From.Id}," +
                       $" FromUsername=@{Message.From.Username}, Text={msgText}))";
            }
        }
    }
}
