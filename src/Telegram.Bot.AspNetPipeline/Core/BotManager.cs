using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConcurrentCollections;
using IRO.Common.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Core.Internal;
using Telegram.Bot.AspNetPipeline.Core.Services;
using Telegram.Bot.AspNetPipeline.Exceptions;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Extensions.Logging;
using Telegram.Bot.AspNetPipeline.Extensions.Session;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Core
{
    /// <summary>
    /// Main object,  that control all processes in <see cref="Telegram.Bot.AspNetPipeline"/>.
    /// </summary>
    public class BotManager : IDisposable
    {
        #region Private fields
        IUpdatesReceiver _updatesReceiver;

        readonly ITelegramBotClient _bot;

        #region Resolved serviced.
        ILogger _logger;

        ILoggerFactory _loggerFactory;
        #endregion

        #region Setup and run data
        readonly object _setupLocker = new object();

        readonly object _runLocker = new object();

        bool _isSetup;

        Action<IPipelineBuilder> _pipelineBuilderAction;

        bool _servicesConfigured;

        ServiceCollectionWrapper _serviceCollectionWrapper;

        IEnumerable<Type> _forDispose;
        #endregion

        /// <summary>
        /// Builded from all other delegates, registered in <see cref="IPipelineBuilder"/>.
        /// </summary>
        UpdateProcessingDelegate _updateProcessingDelegate;

        /// <summary>
        /// UpdateContext from pending tasks.
        /// </summary>
        readonly ConcurrentHashSet<UpdateContext> _pendingUpdateContexts = new ConcurrentHashSet<UpdateContext>();
        #endregion

        #region Properties
        public IServiceProvider Services { get; private set; }

        /// <summary>
        /// Initialized on setup.
        /// </summary>
        public BotClientContext BotContext { get; private set; }

        public bool IsRunning { get; private set; }

        #region Resolved services.
        /// <summary>
        /// Private and here too. Resolved from ServiceCollection.
        /// </summary>
        [Obsolete]
        LoggingAdvancedOptions LoggingAdvancedOptions { get; set; }

        /// <summary>
        /// Check if update context pending.
        /// Default service is <see cref="CreationTimePendingExceededChecker"/> with 30 minutes delay.
        /// Can set your handler in <see cref="BotManager.ConfigureServices"/>.
        /// </summary>
        public IPendingExceededChecker PendingExceededChecker { get; private set; }

        /// <summary>
        /// Aka thread manager.
        /// Can set your handler in ConfigureServices().
        /// </summary>
        public IExecutionManager ExecutionManager { get; private set; }
        #endregion
        #endregion

        /// <summary>
        /// If you want to use same <see cref="IServiceProvider"/> in ASP.NET and <see cref="Telegram.Bot.AspNetPipeline"/>
        /// you can user ServiceProviderBuilderDelegate. Pass here ASP.NET IServiceCollection (to register there default and yours
        /// services) and pass builded <see cref="IServiceProvider"/> to <see cref="BotManager.Setup"/>.
        /// <para></para>
        /// If you will use same <see cref="IServiceCollection"/> for two or more <see cref="BotManager"/>s it probably will broke
        /// half of middleware.
        /// </summary>
        public BotManager(
            ITelegramBotClient bot,
            IServiceCollection servicesCollection = null
            )
        {
            if (bot == null)
                throw new ArgumentNullException(nameof(bot));
            _bot = bot;
            servicesCollection = servicesCollection ?? new ServiceCollection();
            _serviceCollectionWrapper = new ServiceCollectionWrapper(servicesCollection);
        }

        /// <summary>
        /// In new version invoked straight away, not in Setup(). Must be invoked once.
        /// <para></para>.
        /// Register services and middleware.
        /// Registration separated from configuring, because all services must be registered before
        /// service container builded.
        /// </summary>
        /// <param name="servicesConfigureAction"></param>
        public void ConfigureServices(Action<ServiceCollectionWrapper> servicesConfigureAction)
        {
            if (_isSetup)
                throw new TelegramAspException($"Can't configure {nameof(BotManager)} after setup.");
            if (servicesConfigureAction == null)
                throw new ArgumentNullException(nameof(servicesConfigureAction));
            if (_servicesConfigured)
                throw new TelegramAspException("Services was configured before.");

            //Register mandatory services.
            AddMandatoryServices(_serviceCollectionWrapper);
            servicesConfigureAction.Invoke(_serviceCollectionWrapper);
            _forDispose = _serviceCollectionWrapper.ForDispose
                .ToArray();
            _servicesConfigured = true;
        }

        /// <summary>
        /// Configure middleware.
        /// <para></para>
        /// Can call bot.StartReceiving() before to configure bot.
        /// </summary>
        public void ConfigureBuilder(Action<IPipelineBuilder> pipelineBuilderAction)
        {
            if (_isSetup)
                throw new TelegramAspException($"Can't configure {nameof(BotManager)} after setup.");
            _pipelineBuilderAction = pipelineBuilderAction
                ?? throw new ArgumentNullException(nameof(pipelineBuilderAction));
        }

        /// <summary>
        /// Call Setup() first to configure IServiceProvider or it will be called automatically.
        /// Subscribe on update events and start receiving.
        /// </summary>
        public void Start()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(BotManager));
            
            lock (_runLocker)
            {
                if (IsRunning)
                    return;
                Setup();
                StartReceivingEvents();
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
                throw new ObjectDisposedException(nameof(BotManager));

            try
            {
                if (!IsRunning)
                    return;

                StopReceivingEvents();

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
                _logger.LogTrace("BotManager stopped.");
            }
            catch (Exception ex)
            {
                throw new TelegramAspException($"Error while stopping {nameof(BotManager)}.");
            }

        }

        /// <summary>
        /// Execute initialization callbacks and build pipeline.
        /// <para></para>
        /// Called automatically on Start() first call.
        /// </summary>
        /// <param name="serviceProvider">
        /// If not null - will use passed provider instead builded from service collection.
        /// Useful when you wan't to set same container in ASP.NET and here.
        /// </param>
        /// <param name="updatesReceiver">
        /// Use it to customize how bot receiving updates.
        /// Default is <see cref="PollingUpdatesReceiver"/> and subscribe on <see cref="ITelegramBotClient.OnUpdate"/>.
        /// It will set webhook string empty, to enable polling.
        /// </param>
        public void Setup(IServiceProvider serviceProvider = null, IUpdatesReceiver updatesReceiver = null)
        {
            //?Why use IUpdatesReceiver? Why not just make ProcessUpdate method public?
            //Was decided to use interface with event to wrote more "infrastructure" code in BotManager.

            lock (_setupLocker)
            {
                try
                {
                    if (_isSetup)
                    {
                        return;
                    }

                    //Init bot context.
                    var botInfo = _bot.GetMeAsync().Result;
                    BotContext = new BotClientContext(_bot, botInfo);

                    //Set UpdateReceiver.
                    _updatesReceiver = updatesReceiver ?? new PollingUpdatesReceiver();
                    _updatesReceiver.Init(this);

                    //Build service provider.
                    string providerIndentifierForLog;
                    if (serviceProvider == null)
                    {
                        Services = _serviceCollectionWrapper.Services.BuildServiceProvider();
                        providerIndentifierForLog = "ServiceProvider builded from ServiceCollection.";
                    }
                    else
                    {
                        Services = serviceProvider;
                        providerIndentifierForLog = "Passed ServiceProvider used.";
                    }
                    _serviceCollectionWrapper = null;

                    //Resolve services needed for BotManager.
                    ResolveBotManagerServices();
                    _logger.LogTrace("Services initialized. " + providerIndentifierForLog);

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
                    var exception = new TelegramAspException("BotManager setup exception.", ex);
                    if (_logger == null)
                    {
                        Debug.WriteLine(exception.ToString());
                    }
                    else
                    {
                        _logger.LogError(exception, "");
                    }
                    throw exception;
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
            //Dispose singletones that registered to be disposed.
            foreach (var type in _forDispose)
            {
                IDisposable serviceToDispose = null;
                try
                {
                    serviceToDispose=Services.GetService(type) as IDisposable;
                }
                catch
                {
                }
                serviceToDispose?.Dispose();
            }
            (Services as IDisposable)?.Dispose();
            Services = null;

            BotContext = null;
            _updateProcessingDelegate = null;
            IsDisposed = true;
            _updatesReceiver.BotManagerDisposed();
            GC.Collect();
            _logger.LogTrace(nameof(BotManager) + " disposed.");
        }
        #endregion

        #region Mandatory middleware and services

        /// <summary>
        /// Here only musthave middleware services.
        /// </summary>
        void AddMandatoryServices(ServiceCollectionWrapper serviceCollectionWrapper)
        {
            serviceCollectionWrapper.AddPendingExceededChecker(
                (serviceProvider) => new CreationTimePendingExceededChecker(TimeSpan.FromMinutes(30))
            );
            serviceCollectionWrapper.AddExecutionManager<ThreadPoolExecutionManager>();
            serviceCollectionWrapper.AddBotExt();
            serviceCollectionWrapper.Services.AddLogging();
            serviceCollectionWrapper.AddExceptionHandling();
            serviceCollectionWrapper.LoggingAdvancedConfigure(new LoggingAdvancedOptions());
            serviceCollectionWrapper.AddRamSessionStorage();
        }

        /// <summary>
        /// Mandatory middleware registered through pipelineBuilder because it simple.
        /// But they have some crunches, that default middleware can't have.
        /// </summary>
        /// <param name="pipelineBuilder"></param>
        void UseMandatoryMiddleware(IPipelineBuilder pipelineBuilder)
        {
            //Exception handling must used before all other.
            pipelineBuilder.UseExceptionHandling();
            pipelineBuilder.UseBotExt();
        }

        void ResolveBotManagerServices()
        {
            _loggerFactory = Services.GetRequiredService<ILoggerFactory>();
            _logger = _loggerFactory.CreateLogger(GetType());
            _logger.LogTrace("Logger initialized in BotManager. BotManager STARTED.");

            PendingExceededChecker = Services.GetRequiredService<IPendingExceededChecker>();
            ExecutionManager = Services.GetRequiredService<IExecutionManager>();
            var loggingAdvancedOptions = Services.GetRequiredService<Func<LoggingAdvancedOptions>>().Invoke();
            loggingAdvancedOptions.LazySerializerFactory = loggingAdvancedOptions.LazySerializerFactory ?? new LazySerializerFactory();
            LoggingAdvancedOptions = loggingAdvancedOptions;
        }
        #endregion

        #region Bot events region
        void StartReceivingEvents()
        {
            _updatesReceiver.UpdateReceived += OnUpdateEventHandler;
            _updatesReceiver.StartReceiving();
        }

        void StopReceivingEvents()
        {
            _updatesReceiver.StopReceiving();
            _updatesReceiver.UpdateReceived -= OnUpdateEventHandler;
        }

        void OnUpdateEventHandler(object sender, UpdateReceivedEventArgs updateEventArgs)
        {
            //Launch pending updates checker.
            AbortPendingExceeded();

            //Processing current update.
            ProcessUpdate(updateEventArgs.Update);
        }
        #endregion

        #region Processing update.
        void ProcessUpdate(Update update)
        {
            Func<Task> processingFunc = async () =>
            {
                try
                {
                    await UpdateProcessingHandler(update);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        "Exception in executed UpdateProcessingDelegate: '{0}'.",
                        ex
                        );
                    throw;
                }
            };
            ExecutionManager.ProcessUpdate(processingFunc);
        }
        #endregion

        async Task UpdateProcessingHandler(Update update)
        {
            UpdateContext pendingUpdateContext = null;
            IServiceScope servicesScope = Services.CreateScope();
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();

                //Create context.
                var updateContext = new UpdateContext(
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
                //Remove from pending list.
                if (pendingUpdateContext != null)
                {
                    pendingUpdateContext.Dispose();
                    _pendingUpdateContexts.TryRemove(pendingUpdateContext);
                    _logger.LogTrace("'{0}' disposed and removed from pending.", pendingUpdateContext);
                }
                servicesScope.Dispose();
            }
        }

        #region Check pending.
        /// <summary>
        /// Delay to check pending requests.
        /// Default is 1 minute.
        /// </summary>
        public TimeSpan CheckPendingDelay = TimeSpan.FromMinutes(1);

        /// <summary>
        /// If pending requests count is more - will start check and aborting pending
        /// requests (only allowed in <see cref="IPendingExceededChecker"/>.
        /// Default value is 100 context instances.
        /// </summary>
        public int MaxPendingContextCount = 100;

        readonly object _checkPendingLocker = new object();

        DateTime _lastPendingCheck = DateTime.Now;

        /// <summary>
        /// Abort timeouted pending requests if there CheckPendingDelay passed or count limit. 
        /// </summary>
        void AbortPendingExceeded()
        {
            if (!IsPendingAbortAllowed())
                return;

            Task.Run(() =>
            {
                lock (_checkPendingLocker)
                {
                    //New check in locker scope.
                    if (!IsPendingAbortAllowed())
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
                    _logger.LogInformation(
                        "Pending check finished. Removed {0}, remained {1}.",
                        removedCount,
                        _pendingUpdateContexts.Count
                    );
                }
            });
        }

        bool IsPendingAbortAllowed()
        {
            return DateTime.Now - _lastPendingCheck > CheckPendingDelay || _pendingUpdateContexts.Count > MaxPendingContextCount;
        }
        #endregion

    }
}
