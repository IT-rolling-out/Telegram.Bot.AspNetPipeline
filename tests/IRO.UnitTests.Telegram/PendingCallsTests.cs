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
using Telegram.Bot.AspNetPipeline.Services.Implementations;
using Telegram.Bot.Types;

namespace IRO.UnitTests.Telegram
{
    public class PendingCallsTests
    {
        IPipelineBuilder _pipelineBuilder;
        UpdateContext _ctx;
        ThreadPoolExecutionManager _executionManager=new ThreadPoolExecutionManager();

        [SetUp]
        public void Setup()
        {
            var servicesCollection = new ServiceCollection();
            var servicesProvider =servicesCollection.BuildServiceProvider();
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
            var locker=new object();
            List<Task> tasks = new List<Task>();
            int count = 200;
            int finishedTasks = 0;
            //Launch many threads.
            for (int i = 0; i < count; i++)
            {
                var t = _executionManager.ProcessUpdate(async () =>
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
            foreach(var t in tasks)
            {
                await t;
            }
            
            Assert.Zero(_executionManager.PendingTasksCount);
            Assert.AreEqual(count, finishedTasks);
        }
    }
}
