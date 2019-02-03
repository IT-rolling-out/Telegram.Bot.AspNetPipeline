using System;
using System.Threading;
using System.Threading.Tasks;
using ConcurrentCollections;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Args;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot;
using Telegram.Bot.AspNetPipeline.Services;
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

        Action<ServiceCollectionWrapper> _servicesConfigureAction;

        ServiceCollectionWrapper _serviceCollectionWrapper;
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

        public BotHandler(
            ITelegramBotClient bot,
            IServiceCollection servicesCollection = null
            )
        {
            if (bot == null)
                throw new ArgumentNullException(nameof(bot));
            BotContext = new BotClientContext(bot);
            servicesCollection = servicesCollection ?? new ServiceCollection();
            _serviceCollectionWrapper =new ServiceCollectionWrapper(servicesCollection);
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
                    await ExecutionManager.AwaitAllPending(TimeSpan.FromMilliseconds(waitPendingMS));
                foreach (var item in _pendingUpdateContexts)
                {
                    var cancellationTokenSource = HiddenUpdateContext.Resolve(item).UpdateProcessingAbortedSource;
                    cancellationTokenSource.Cancel();
                }

                //Wait after cancel.
                if (waitCancellationMS > 0)
                    await ExecutionManager.AwaitAllPending(TimeSpan.FromMilliseconds(waitCancellationMS));
                //Dispose.
                foreach (var item in _pendingUpdateContexts)
                {
                    item.Dispose();
                }

                _pendingUpdateContexts.Clear();
                IsRunning = false;

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
