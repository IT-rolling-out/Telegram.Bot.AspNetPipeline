using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConcurrentCollections;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Args;
using Telegram.Bot.AspNetPipeline.Core.Services;
using Telegram.Bot.AspNetPipeline.Implementations;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Core.Builder
{

    public class BotHandler : IDisposable
    {
        #region Setup properties
        bool _isSetup;

        Action<IPipelineBuilder> _pipelineBuilderAction;

        Action<IServiceCollection> _servicesConfigureAction;

        IServiceCollection _serviceCollection;
        #endregion

        UpdateProcessingDelegate _updateProcessingDelegate;

        /// <summary>
        /// UpdateContext from pending tasks.
        /// </summary>
        ConcurrentHashSet<UpdateContext> _pendingUpdateContexts = new ConcurrentHashSet<UpdateContext>();

        public IServiceProvider Services { get; private set; }

        public BotClientContext BotContext { get; }

        public IPendingExceededChecker PendingExceededChecker { get; }

        /// <summary>
        /// Aka thread manager.
        /// </summary>
        public IExecutionManager ExecutionManager { get; }

        /// <summary>
        /// </summary>
        /// <param name="pendingExceededChecker">Limit for command execution.
        /// Default is <see cref="CreationTimePendingExceededChecker"/> with 30 minutes.
        /// </param>
        public BotHandler(
            ITelegramBotClient bot,
            IServiceCollection servicesCollection = null,
            IExecutionManager executionManager = null,
            IPendingExceededChecker pendingExceededChecker = null
            )
        {
            ExecutionManager = executionManager ?? new ThreadPoolExecutionManager();
            BotContext = new BotClientContext(bot);
            _serviceCollection = servicesCollection ?? new ServiceCollection();
            PendingExceededChecker = pendingExceededChecker
               ?? new CreationTimePendingExceededChecker(TimeSpan.FromMinutes(30));
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
                throw new Exception("Services configure action was setted before");
            _servicesConfigureAction = servicesConfigureAction;
        }

        public void ConfigureBuilder(Action<IPipelineBuilder> pipelineBuilderAction)
        {
            if (pipelineBuilderAction == null)
                throw new ArgumentNullException(nameof(pipelineBuilderAction));
            if (_pipelineBuilderAction != null)
                throw new Exception("Pipeline builder action was setted before");
            _pipelineBuilderAction = pipelineBuilderAction;
        }

        public void Setup()
        {
            if (_isSetup)
            {
                throw new Exception("You can setup BotHandler twice.");
            }

            //Register services.
            RegisterMandatoryServices(_serviceCollection);
            _servicesConfigureAction?.Invoke(_serviceCollection);
            _servicesConfigureAction = null;
            Services = _serviceCollection.BuildServiceProvider();
            _serviceCollection = null;

            //Not implemented.
            IPipelineBuilder pipelineBuilder = new PipelineBuilder(Services);

            //Register middleware.
            RegisterMandatoryMiddleware(pipelineBuilder);
            _pipelineBuilderAction?.Invoke(pipelineBuilder);
            _pipelineBuilderAction = null;

            //Build pipeline.
            _updateProcessingDelegate = pipelineBuilder.Build();

            //Setup event handlers.
            SubscribeBotEvents();

            _isSetup = true;
        }

        public void Dispose()
        {
            UnsubscribeBotEvents();
        }

        /// <summary>
        /// Here only musthave middleware services.
        /// </summary>
        /// <param name="serviceCollection"></param>
        void RegisterMandatoryServices(IServiceCollection serviceCollection)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Mandatory middleware registered through pipelineBuilder because it simple.
        /// But they have some crunches, that default middleware cant have.
        /// </summary>
        /// <param name="pipelineBuilder"></param>
        void RegisterMandatoryMiddleware(IPipelineBuilder pipelineBuilder)
        {
            throw new NotImplementedException();
        }

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
                            cancellationTokenSource.Token,
                            null
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
                        _pendingUpdateContexts.TryRemove(pendingUpdateContext);
                    }
                }
            };

            ExecutionManager.ProcessUpdate(processingFunc);
        }

        void AbortPendingExceeded()
        {
            foreach (var ctx in _pendingUpdateContexts)
            {
                //If pending time limit exceeded.
                if (PendingExceededChecker.IsPendingExceeded(ctx))
                {
                    _pendingUpdateContexts.TryRemove(ctx);
                }
            }
        }

    }
}
