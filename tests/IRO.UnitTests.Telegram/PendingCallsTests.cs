using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Telegram.Bot;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Services;
using Telegram.Bot.Types;

namespace IRO.UnitTests.Telegram
{
    public class PendingCallsTests
    {
        IPipelineBuilder _pipelineBuilder;
        UpdateContext _ctx;


        [SetUp]
        public void Setup()
        {
            var servicesCollection = new ServiceCollection();
            var servicesProvider = servicesCollection.BuildServiceProvider();
            _pipelineBuilder = new PipelineBuilder(
                servicesProvider
                );
            var bot = new Mock<ITelegramBotClient>();
            var update = new Mock<Update>();
            var cts = new CancellationTokenSource();
            _ctx = new UpdateContext(
                update.Object,
                new BotClientContext(bot.Object),
                servicesProvider,
                cts.Token
            );
        }

        [Test]
        public async Task NoPending()
        {
            var executionManager = new ThreadPoolExecutionManager();
            object locker = new object();
            List<Task> tasks = new List<Task>();
            int count = 50;
            int finishedTasks = 0;
            //Launch many threads.
            for (int i = 0; i < count; i++)
            {
                var t = executionManager.ProcessUpdate(async () =>
                {
                    Thread.Sleep(1000);
                    lock (locker)
                    {
                        finishedTasks++;
                    }
                });
                tasks.Add(t);
            }

            //Await all.
            await executionManager.AwaitAllPending();

            Assert.Zero(executionManager.PendingTasksCount);
            Assert.AreEqual(count, finishedTasks);
        }

        [Test]
        public async Task AllPending_ThreadLock()
        {
            var executionManager = new ThreadPoolExecutionManager();
            var locker = new object();
            List<Task> tasks = new List<Task>();
            int count = 200;
            int finishedTasks = 0;
            Semaphore are = new Semaphore(0, int.MaxValue);

            //Launch many threads.
            for (int i = 0; i < count; i++)
            {
                var t = executionManager.ProcessUpdate(async () =>
                {
                    are.WaitOne();
                    finishedTasks++;
                });
                tasks.Add(t);
            }

            var pendingTasksCount = executionManager.PendingTasksCount;
            var finishedTasksRes = finishedTasks;

            //Await all.
            await executionManager.AwaitAllPending(TimeSpan.FromMilliseconds(100));

            //Release threads.
            for (int i = 0; i < count; i++)
            {
                are.Release();
            }
            await executionManager.AwaitAllPending();

            Assert.AreEqual(count, pendingTasksCount);
            Assert.Zero(finishedTasksRes);
        }


        [Test]
        public async Task AllPending_TaskAwait()
        {
            var executionManager = new ThreadPoolExecutionManager();
            var locker = new object();
            List<Task> tasks = new List<Task>();
            int count = 200;
            int finishedTasks = 0;
            List<TaskCompletionSource<object>> tasksSources = new List<TaskCompletionSource<object>>();

            //Launch many threads.
            for (int i = 0; i < count; i++)
            {
                var t = executionManager.ProcessUpdate(async () =>
                {
                    var tcs = new TaskCompletionSource<object>();
                    lock (tasksSources)
                    {
                        tasksSources.Add(tcs);
                    }
                    await tcs.Task;
                    finishedTasks++;
                });
                tasks.Add(t);
            }

            var pendingTasksCount = executionManager.PendingTasksCount;
            var finishedTasksRes = finishedTasks;

            //Await all.
            await executionManager.AwaitAllPending(TimeSpan.FromMilliseconds(100));

            //Release threads.
            lock (tasksSources)
            {
                for (int i = 0; i < count; i++)
                {
                    //If we not "release all task - threads will not been locked and application will not freeze.
                    //Thats how Task works.
                    tasksSources[i].SetCanceled();
                }
            }

            await executionManager.AwaitAllPending();

            Assert.AreEqual(count, pendingTasksCount);
            Assert.Zero(finishedTasksRes);
        }
    }
}
