using System;
using System.Threading.Tasks;
using IRO.Common.Abstractions;
using IRO.Mvc.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Telegram.Bot.Args;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Core.Services;
using Telegram.Bot.AspNetPipeline.Exceptions;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.WebhookSupport
{
    /// <summary>
    /// You can use many webhooks (one per bot) on one ASP.NET pipeline.
    /// </summary>
    public class WebhookUpdatesReceiver : IUpdatesReceiver, IInformativeDisposable
    {
        #region When created fields.
        readonly string _domain;

        /// <summary>
        /// Default is 'template/update/{0}' (username of bot will be pasted).
        /// </summary>
        readonly string _pathTemplate;

        readonly bool _setWebhookAutomatically;
        #endregion

        #region When initialized fields.
        bool _isInit;

        BotManager _botManager;

        /// <summary>
        /// Something like 'template/update/my_cool_bot'.
        /// </summary>
        string _pathWithBotname;

        /// <summary>
        /// Return null if you call it before initialization.
        /// </summary>
        public string WebhookFullUrl { get; private set; }
        #endregion

        /// <summary>
        /// Settings to parse update.
        /// </summary>
        public JsonSerializerSettings JsonSettings { get; set; } = new JsonSerializerSettings();

        public bool IsReceiving { get; private set; }

        public event UpdateReceivedEvent UpdateReceived;

        WebhookUpdatesReceiver(string domain, string pathTemplate, bool setWebhookAutomatically)
        {
            if (string.IsNullOrEmpty(domain))
                throw new TelegramAspException("Domain path string can't be null or empty.");
            if (string.IsNullOrEmpty(pathTemplate))
                throw new TelegramAspException("Path template string can't be null or empty.");

            _pathTemplate = pathTemplate;
            _setWebhookAutomatically = setWebhookAutomatically;

            domain = domain.Trim();
            if (!domain.EndsWith("/"))
                domain += "/";
            _domain = domain;
        }

        /// <summary>
        /// You must set webhook url for <see cref="ITelegramBotClient"/> yourself.
        /// </summary>
        /// <param name="pathTemplate">Not all url, just part after domain (HttpContext.Path in ASP.NET).</param>
        /// <param name="setWebhookAutomatically">
        /// If true - will call SetWebhook after initialization.
        /// <para></para>
        /// If false - you must call it yourself after <see cref="BotManager.Setup"/>
        /// and pass <see cref="WebhookFullUrl"/> as url of webhook.
        /// Useful when you want configure webhook.
        /// </param>
        public static WebhookUpdatesReceiver Create(
            IApplicationBuilder app,
            string domain,
            string pathTemplate = "telegram/update/{0}",
            bool setWebhookAutomatically = true
        )
        {
            var receiver = new WebhookUpdatesReceiver(
                domain,
                pathTemplate,
                setWebhookAutomatically
            );
            app.Use(receiver.HttpRequestsHandler);
            return receiver;
        }

        public void StartReceiving()
        {
            ThrowIfNeeded();
            IsReceiving = true;
        }

        public void StopReceiving()
        {
            ThrowIfNeeded();
            IsReceiving = false;
        }

        /// <summary>
        /// Used as initializer. Invoked in <see cref="BotManager.Setup"/>.
        /// </summary>
        public void Init(BotManager botManager)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
            if (_isInit)
                throw new TelegramAspException(GetType().Name + " was init before.");
            _botManager = botManager
                          ?? throw new ArgumentNullException(nameof(botManager));

            PrepareUrl(botManager);
            var bot = botManager.BotContext.Bot;
            if (_setWebhookAutomatically)
                bot.SetWebhookAsync(WebhookFullUrl).Wait();
            _isInit = true;
        }

        public void BotManagerDisposed()
        {
            Dispose();
        }

        #region Dispose region.
        public bool IsDisposed { get; private set; }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            if (IsDisposed)
                return;
            try
            {
                StopReceiving();
            }
            catch { }
            _botManager = null;
            IsDisposed = true;
        }
        #endregion

        void ThrowIfNeeded()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
            if (!_isInit)
                throw new TelegramAspException(GetType().Name + " not init.");
        }

        /// <summary>
        /// Called after setup.
        /// </summary>
        void PrepareUrl(BotManager botManager)
        {
            var bot = botManager.BotContext.Bot;
            var username = botManager.BotContext.Username;
            _pathWithBotname = string.Format(_pathTemplate, username);
            WebhookFullUrl = _domain + _pathWithBotname;

        }

        async Task HttpRequestsHandler(HttpContext ctx, Func<Task> next)
        {
            var path = ctx.Request.Path.ToString();
            if (!IsDisposed && _isInit && path.Contains(_pathWithBotname))
            {
                //When webhook of current bot.
                var requestBodyStr = await ctx.GetRequestBodyText();
                var update=JsonConvert.DeserializeObject<Update>(requestBodyStr, JsonSettings);
                var args = new UpdateReceivedEventArgs(update);
                UpdateReceived?.Invoke(this, args);
            }
            else
            {
                await next();
            }
        }
    }
}