using System;
using System.Threading;
using System.Threading.Tasks;
using ConcurrentCollections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Args;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot;
using Telegram.Bot.AspNetPipeline.Extensions.Logging;
using Telegram.Bot.AspNetPipeline.Extensions.Serialization;
using Telegram.Bot.AspNetPipeline.Services;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Builder
{
    public class BotHandler : IDisposable
    {
        #region Private fields
        #region Resolved serviced.
        ILogger _logger;

        ILoggerFactory _loggerFactory;
        #endregion

        #region Setup and run data
        readonly object _setupLocker = new object();

        readonly object _runLocker = new object();

        bool _isSetup;

        Action<IPipelineBuilder> _pipelineBuilderAction;

        Action<ServiceCollectionWrapper> _servicesConfigureAction;

        ServiceCollectionWrapper _serviceCollectionWrapper;
        #endregion

        UpdateProcessingDelegate _updateProcessingDelegate;

        /// <summary>
        /// UpdateContext from pending tasks.
        /// </summary>
        readonly ConcurrentHashSet<UpdateContext> _pendingUpdateContexts = new ConcurrentHashSet<UpdateContext>();
        #endregion

        #region Properties
        public IServiceProvider Services { get; private set; }

        public BotClientContext BotContext { get; private set; }

        public bool IsRunning { get; private set; }

        #region Resolved services.
        /// <summary>
        /// Private and here too. Resolved from ServiceCollection.
        /// </summary>
        LoggingAdvancedOptions LoggingAdvancedOptions { get; set; }

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
        #endregion

        public BotHandler(
            ITelegramBotClient bot,
            IServiceCollection servicesCollection = null
            )
        {
            if (bot == null)
                throw new ArgumentNullException(nameof(bot));
            BotContext = new BotClientContext(bot);
            servicesCollection = servicesCollection ?? new ServiceCollection();
            _serviceCollectionWrapper = new ServiceCollectionWrapper(servicesCollection);
        }

        /// <summary>
        /// Recommended to use current method for service registration to register it before any middleware.
        /// </summary>
        /// <param name="servicesConfigureAction"></param>
        public void ConfigureServices(Action<ServiceCollectionWrapper> servicesConfigureAction)
        {
            _servicesConfigureAction = servicesConfigureAction
                ?? throw new ArgumentNullException(nameof(servicesConfigureAction));
        }

        public void ConfigureBuilder(Action<IPipelineBuilder> pipelineBuilderAction)
        {
            _pipelineBuilderAction = pipelineBuilderAction
               ?? throw new ArgumentNullException(nameof(pipelineBuilderAction));
        }

        public void Start()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(BotHandler));

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

        /// <summary>
        /// </summary>
        /// <param name="waitPendingMS">Time to wait before cancellation.</param>
        /// <param name="waitCancellationMS">Time to wait after cancellation.
        /// Default is one second to allow threads cancel work normally before UpdateContext disposed.</param>
        /// <returns>Task to await pending tasks.</returns>
        public async Task Stop(int waitPendingMS = 0, int waitCancellationMS = 1000)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(BotHandler));

            try
            {
                if (!IsRunning)
                    return;

                UnsubscribeBotEvents();
                //Wait before cancel.
                if (waitPendingMS > 0)
                {
                    _logger.LogTrace("Waiting pending normally finished.");
                    await ExecutionManager.AwaitAllPending(TimeSpan.FromMilliseconds(waitPendingMS));
                }

                foreach (var item in _pendingUpdateContexts)
                {
                    var cancellationTokenSource = item.HiddenContext().UpdateProcessingAbortedSource;
                    cancellationTokenSource.Cancel();
                }

                //Wait after cancel.
                if (waitCancellationMS > 0)
                {
                    _logger.LogTrace("Waiting cancellation.");
                    await ExecutionManager.AwaitAllPending(TimeSpan.FromMilliseconds(waitCancellationMS));
                }

                //Dispose.
                foreach (var item in _pendingUpdateContexts)
                {
                    item.Dispose();
                }

                _pendingUpdateContexts.Clear();
                IsRunning = false;
                _logger.LogTrace("BotHandler stopped.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while stopping {nameof(BotHandler)}.");
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
                    AddMandatoryServices(_serviceCollectionWrapper);
                    if (_servicesConfigureAction == null)
                    {
                        throw new NullReferenceException(
                            "Can`t init without services configure action. " +
                            "Probably ConfigureServices wasn't invoked."
                        );
                    }
                    _servicesConfigureAction.Invoke(_serviceCollectionWrapper);
                    _servicesConfigureAction = null;
                    Services = _serviceCollectionWrapper.Services.BuildServiceProvider();
                    _serviceCollectionWrapper = null;

                    //Resolve services needed for BotHandler.
                    ResolveBotHandlerServices();
                    _logger.LogTrace("Services initialized.");

                    //Not implemented.
                    IPipelineBuilder pipelineBuilder = new PipelineBuilder(Services);

                    //Register middleware.
                    UseMandatoryMiddleware(pipelineBuilder);
                    if (_pipelineBuilderAction == null)
                    {
                        throw new NullReferenceException(
                            "Can`t init without pipeline builder configure action. " +
                            "Probably ConfigureBuilder wasn't invoked."
                        );
                    }
                    _pipelineBuilderAction.Invoke(pipelineBuilder);
                    _pipelineBuilderAction = null;

                    //Build pipeline.
                    _updateProcessingDelegate = pipelineBuilder.Build();
                    _isSetup = true;
                    _logger.LogTrace("Pipeline builded.");
                }
                catch (Exception ex)
                {
                    throw new Exception("BotHandler setup exception.", ex);
                }
            }
        }

        #region Dispose region.
        public bool IsDisposed { get; private set; }

        public async void Dispose()
        {
            if (IsDisposed)
                return;
            await Stop();
            Services = null;
            BotContext = null;
            _updateProcessingDelegate = null;
            IsDisposed = true;
            GC.Collect();
            _logger.LogTrace("BotHandler disposed.");
        }
        #endregion

        #region Mandatory middleware and services

        /// <summary>
        /// Here only musthave middleware services.
        /// </summary>
        /// <param name="serviceCollection"></param>
        void AddMandatoryServices(ServiceCollectionWrapper serviceCollectionWrapper)
        {
            serviceCollectionWrapper.AddPendingExceededChecker(
                (serviceProvider) => new CreationTimePendingExceededChecker(TimeSpan.FromMinutes(30))
            );
            serviceCollectionWrapper.AddExecutionManager<ThreadPoolExecutionManager>();
            serviceCollectionWrapper.AddBotExt();
            serviceCollectionWrapper.Services.AddLogging();

            //Logging crunch. More info in LoggingAdvancedOptions.
            //I really can't find better way to serialize UpdateContext to json if logging it and keep it optimized.
            //So i use LazySerializer and current boolean value as switch. If there no loggers or if current value is false, then
            //!UpdateContext WILL NOT be seriaalized to json.
            //I use HiddenUpdateContext and extension method GetLoggerScope to return LazySerializer if true and UpdateContext if false.
            serviceCollectionWrapper.LoggingAdvancedConfigure(new LoggingAdvancedOptions());
        }

        /// <summary>
        /// Mandatory middleware registered through pipelineBuilder because it simple.
        /// But they have some crunches, that default middleware can't have.
        /// </summary>
        /// <param name="pipelineBuilder"></param>
        void UseMandatoryMiddleware(IPipelineBuilder pipelineBuilder)
        {
            pipelineBuilder.UseBotExt();
        }

        void ResolveBotHandlerServices()
        {
            _loggerFactory = Services.GetService<ILoggerFactory>();
            _logger = _loggerFactory.CreateLogger(GetType());
            _logger.LogTrace("Logger initialized in BotHandler. BOTHANDLER STARTED.");

            PendingExceededChecker = Services.GetService<IPendingExceededChecker>();
            ExecutionManager = Services.GetService<IExecutionManager>();
            var lao = Services.GetService<Func<LoggingAdvancedOptions>>().Invoke();
            lao.LazySerializerFactory = lao.LazySerializerFactory ?? new LazySerializerFactory();
            LoggingAdvancedOptions = lao;
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
                IServiceScope servicesScope = Services.CreateScope();
                try
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
                        DateTime.Now,
                        LoggingAdvancedOptions
                    );
                    updateContext.Properties[HiddenUpdateContext.DictKeyName] = hiddenUpdateContext;

                    _logger.LogTrace("'{0}' created.", updateContext);

                    //Add to pending list.
                    _pendingUpdateContexts.Add(
                        updateContext
                    );
                    pendingUpdateContext = updateContext;

                    //Execute.
                    await _updateProcessingDelegate.Invoke(updateContext, async () => { });
                }
                finally
                {
                    try
                    {
                        //Remove from pending list.
                        if (pendingUpdateContext != null)
                        {
                            //throw new Exception("EEEEEEEEEEEEEEEEEEEE");
                            pendingUpdateContext.Dispose();
                            _pendingUpdateContexts.TryRemove(pendingUpdateContext);
                            _logger.LogTrace("'{0}' disposed and removed from pending.", pendingUpdateContext);
                        }
                        servicesScope.Dispose();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogTrace("'{0}' removing exception '{1}'.", pendingUpdateContext, ex);
                        throw;
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

                    _logger.LogTrace("Pending check started.");
                    int removedCount = 0;
                    foreach (var ctx in _pendingUpdateContexts)
                    {
                        //If pending time limit exceeded.
                        if (PendingExceededChecker.IsPendingExceeded(ctx) || ctx.IsDisposed)
                        {
                            removedCount++;
                            _pendingUpdateContexts.TryRemove(ctx);
                            //All dispose operations in separate execution context (probably separate thread).
                            ExecutionManager.ProcessUpdate(async () =>
                            {
                                ctx.Dispose();
                            });
                        }
                    }
                    _lastPendingCheck = DateTime.Now;
                    _logger.LogTrace(
                        "Pending check finished. Removed {0}, remained {1}.",
                        removedCount,
                        _pendingUpdateContexts.Count
                    );
                }
            });
        }
        #endregion

    }
}
