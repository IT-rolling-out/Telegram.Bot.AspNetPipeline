using System;
using System.Threading;
using System.Threading.Tasks;
using ConcurrentCollections;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Args;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot;
using Telegram.Bot.AspNetPipeline.Services;
using Telegram.Bot.AspNetPipeline.Services.Implementations;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Builder
{

    public class BotHandler : IDisposable
    {
        #region Setup and run data
        readonly object _setupLocker = new object();

        readonly object _runLocker = new object();

        bool _isSetup;

        Action<IPipelineBuilder> _pipelineBuilderAction;

        Action<IServiceCollection> _servicesConfigureAction;

        IServiceCollection _serviceCollection;
        #endregion

        UpdateProcessingDelegate _updateProcessingDelegate;

        /// <summary>
        /// UpdateContext from pending tasks.
        /// </summary>
        readonly ConcurrentHashSet<UpdateContext> _pendingUpdateContexts = new ConcurrentHashSet<UpdateContext>();

        public IServiceProvider Services { get; private set; }

        public BotClientContext BotContext { get; private set; }

        public bool IsRunning { get; private set; }

        #region Resolved services.
        /// <summary>
        /// Check if service pending.
        /// Default service is <see cref="CreationTimePendingExceededChecker"/> with 30 minutes delay.
        /// Can set your handler in ConfigureServices().
        /// </summary>
        public IPendingExceededChecker PendingExceededChecker { get; private set; }

        /// <summary>
        /// Aka thread manager.
        /// Can set your handler in ConfigureServices().
        /// </summary>
        public IExecutionManager ExecutionManager { get; private set; }

        #endregion

        /// <summary>
        /// </summary>
        /// </param>
        public BotHandler(
            ITelegramBotClient bot,
            IServiceCollection servicesCollection = null
            )
        {
            if (bot == null)
                throw new ArgumentNullException(nameof(bot));
            BotContext = new BotClientContext(bot);
            _serviceCollection = servicesCollection ?? new ServiceCollection();
        }

        /// <summary>
        /// Recommended to use current method for service registration to register it before any middleware.
        /// </summary>
        /// <param name="servicesConfigureAction"></param>
        public void ConfigureServices(Action<IServiceCollection> servicesConfigureAction)
        {
            if (servicesConfigureAction == null)
                throw new ArgumentNullException(nameof(servicesConfigureAction));
            if (_servicesConfigureAction != null)
                throw new Exception("Services configure action was set before");
            _servicesConfigureAction = servicesConfigureAction;
        }

        public void ConfigureBuilder(Action<IPipelineBuilder> pipelineBuilderAction)
        {
            if (pipelineBuilderAction == null)
                throw new ArgumentNullException(nameof(pipelineBuilderAction));
            if (_pipelineBuilderAction != null)
                throw new Exception("Pipeline builder action was set before");
            _pipelineBuilderAction = pipelineBuilderAction;
        }

        public void Start()
        {
            if (IsDisposed)
                throw new ObjectDisposedException("BotHandler");

            lock (_runLocker)
            {
                if (IsRunning)
                    return;
                Setup();
                BotContext.Bot.StartReceiving();
                SubscribeBotEvents();
                IsRunning = true;
            }

        }

        public void Stop()
        {
            if (IsDisposed)
                throw new ObjectDisposedException("BotHandler");

            lock (_runLocker)
            {
                if (!IsRunning)
                    return;
                UnsubscribeBotEvents();
                foreach (var item in _pendingUpdateContexts)
                {
                    item.Dispose();
                }
                _pendingUpdateContexts.Clear();
                IsRunning = false;
            }

        }

        /// <summary>
        /// Execute initialization callbacks and build pipeline.
        /// <para></para>
        /// Called automatically in Start().
        /// </summary>
        public void Setup()
        {
            lock (_setupLocker)
            {
                try
                {
                    if (_isSetup)
                    {
                        return;
                    }

                    //Register services.
                    RegisterMandatoryServices(_serviceCollection);
                    _servicesConfigureAction?.Invoke(_serviceCollection);
                    _servicesConfigureAction = null;
                    Services = _serviceCollection.BuildServiceProvider();
                    _serviceCollection = null;

                    //Resolve services needed for BotHandler.
                    ResolveBotHandlerServices();

                    //Not implemented.
                    IPipelineBuilder pipelineBuilder = new PipelineBuilder(Services);

                    //Register middleware.
                    RegisterMandatoryMiddleware(pipelineBuilder);
                    _pipelineBuilderAction?.Invoke(pipelineBuilder);
                    _pipelineBuilderAction = null;

                    //Build pipeline.
                    _updateProcessingDelegate = pipelineBuilder.Build();
                    _isSetup = true;
                }
                catch (Exception ex)
                {
                    throw new Exception("BotHandler setup exception.", ex);
                }
            }
        }

        #region Dispose region.
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;
            Stop();
            Services = null;
            BotContext = null;
            _updateProcessingDelegate = null;
            IsDisposed = true;
            GC.Collect();
        }
        #endregion

        #region Mandatory middleware and services
        /// <summary>
        /// Here only musthave middleware services.
        /// </summary>
        /// <param name="serviceCollection"></param>
        void RegisterMandatoryServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IPendingExceededChecker>(
                (serviceProvider) => new CreationTimePendingExceededChecker(TimeSpan.FromMinutes(30))
                );
            serviceCollection.AddSingleton<IExecutionManager, ThreadPoolExecutionManager>();

            serviceCollection.AddBotExt();
        }

        /// <summary>
        /// Mandatory middleware registered through pipelineBuilder because it simple.
        /// But they have some crunches, that default middleware cant have.
        /// </summary>
        /// <param name="pipelineBuilder"></param>
        void RegisterMandatoryMiddleware(IPipelineBuilder pipelineBuilder)
        {
            pipelineBuilder.UseBotExt();
        }

        void ResolveBotHandlerServices()
        {
            PendingExceededChecker = Services.GetService<IPendingExceededChecker>();
            ExecutionManager = Services.GetService<IExecutionManager>();
        }
        #endregion

        #region Bot events region
        void SubscribeBotEvents()
        {
            BotContext.Bot.OnUpdate += OnUpdateEventHandler;
        }

        void UnsubscribeBotEvents()
        {
            BotContext.Bot.OnUpdate -= OnUpdateEventHandler;
        }

        void OnUpdateEventHandler(object sender, UpdateEventArgs updateEventArgs)
        {
            //Launc pending updates checker.
            AbortPendingExceeded();

            //Processing current update.
            ProcessUpdate(updateEventArgs.Update);
        }
        #endregion

        void ProcessUpdate(Update update)
        {
            Func<Task> processingFunc = async () =>
            {
                UpdateContext pendingUpdateContext = null;
                try
                {
                    using (var servicesScope = Services.CreateScope())
                    {
                        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

                        //Create context.
                        UpdateContext updateContext = new UpdateContext(
                            update,
                            BotContext,
                            servicesScope.ServiceProvider,
                            cancellationTokenSource.Token
                        );

                        //Create hidden context.
                        var hiddenUpdateContext = new HiddenUpdateContext(
                            cancellationTokenSource,
                            DateTime.Now
                        );
                        updateContext.Properties[HiddenUpdateContext.DictKeyName] = hiddenUpdateContext;

                        //Add to pending list.
                        _pendingUpdateContexts.Add(
                            updateContext
                        );

                        pendingUpdateContext = updateContext;

                        //Execute.
                        await _updateProcessingDelegate.Invoke(updateContext, async () => { });
                    }
                }
                finally
                {
                    //Remove from pending list.
                    if (pendingUpdateContext != null)
                    {
                        pendingUpdateContext.Dispose();
                        _pendingUpdateContexts.TryRemove(pendingUpdateContext);
                    }
                }
            };

            ExecutionManager.ProcessUpdate(processingFunc);
        }

        #region Check pending.
        TimeSpan _checkPendingDelay = TimeSpan.FromMinutes(1);

        readonly object _checkPendingLocker = new object();

        DateTime _lastPendingCheck = DateTime.Now;

        void AbortPendingExceeded()
        {
            if (DateTime.Now - _lastPendingCheck < _checkPendingDelay)
                return;

            Task.Run(() =>
            {
                lock (_checkPendingLocker)
                {
                    if (DateTime.Now - _lastPendingCheck < _checkPendingDelay)
                        return;


                    foreach (var ctx in _pendingUpdateContexts)
                    {
                        //If pending time limit exceeded.
                        if (PendingExceededChecker.IsPendingExceeded(ctx) || ctx.IsDisposed)
                        {
                            _pendingUpdateContexts.TryRemove(ctx);
                            //All dispose operations in separate execution context (probably separate thread).
                            ExecutionManager.ProcessUpdate(async () =>
                            {
                                ctx.Dispose();

                            });
                        }
                    }

                    _lastPendingCheck = DateTime.Now;
                }
            });
        }
        #endregion

    }
}
