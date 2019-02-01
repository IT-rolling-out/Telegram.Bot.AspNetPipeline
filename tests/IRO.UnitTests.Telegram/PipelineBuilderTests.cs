using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Telegram.Bot;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.Types;

namespace IRO.UnitTests.Telegram
{
    public class PipelineBuilderTests
    {
        IPipelineBuilder _pipelineBuilder;
        UpdateContext _ctx;

        [SetUp]
        public void Setup()
        {
            var serv = new ServiceCollection().BuildServiceProvider();
            _pipelineBuilder = new PipelineBuilder(
                serv
                );
            var bot = new Mock<ITelegramBotClient>();
            var update = new Mock<Update>();
            var cts = new CancellationTokenSource();
            _ctx = new UpdateContext(
                update.Object,
                new BotClientContext(bot.Object),
                serv,
                cts.Token
            );
        }

        [Test]
        public async Task TestMiddlewareOrder()
        {
            StringBuilder sb = new StringBuilder();
            _pipelineBuilder.Use(async (ctx, next) =>
            {
                sb.AppendLine("  Middleware 1");
                await next();
            });
            _pipelineBuilder.Use(async (ctx, next) =>
            {
                sb.AppendLine("  Middleware 2");
                await next();
            });
            _pipelineBuilder.Use(async (ctx, next) =>
            {
                sb.AppendLine("  Middleware 3");
                await next();
            });
            _pipelineBuilder.Use(async (ctx, next) =>
            {
                sb.AppendLine("  Middleware 4");
                await next();
            });
            _pipelineBuilder.Use(async (ctx, next) =>
            {
                sb.AppendLine("  Middleware 5");
                await next();
            });

            var func = _pipelineBuilder.Build();
            await func.Invoke(_ctx, async () => { });

            //Just test! I know its stupid.
            var testInfo = sb.ToString();
            var str = RemoveWhiteSpace(testInfo);
            if ("12345" != str)
            {
                Assert.Fail("Expected order: 1-2-3-4-5.\n" +
                            "Middleware execution order:\n" +
                            testInfo);
            }
            else
            {
                Assert.Pass();
            }

        }

        [Test]
        public async Task TestMiddlewareNextNotCalled()
        {
            StringBuilder sb = new StringBuilder();
            _pipelineBuilder.Use(async (ctx, next) =>
            {
                sb.AppendLine("  Middleware 1");
                await next();
            });
            _pipelineBuilder.Use(async (ctx, next) =>
            {
                sb.AppendLine("  Middleware 2");
            });
            _pipelineBuilder.Use(async (ctx, next) =>
            {
                sb.AppendLine("  Middleware 3");
                await next();
            });

            var func = _pipelineBuilder.Build();
            await func.Invoke(_ctx, async () => { });

            //Just test! I know its stupid.
            var testInfo = sb.ToString();
            var str = RemoveWhiteSpace(testInfo);
            if ("12" != str)
            {
                Assert.Fail("Expected order: 1-2.\n" +
                            "Middleware execution order:\n" +
                            testInfo);
            }
            else
            {
                Assert.Pass();
            }

        }

        [Test]
        public async Task TestMiddlewareProcessed()
        {
            StringBuilder sb = new StringBuilder();
            _pipelineBuilder.Use(async (ctx, next) =>
            {
                sb.AppendLine("  Middleware 1");
                ctx.Processed();
                await next();
            });
            _pipelineBuilder.Use(async (ctx, next) =>
            {
                if (!ctx.IsProcessed)
                    sb.AppendLine("  Middleware 2");
                await next();
            });
            _pipelineBuilder.Use(async (ctx, next) =>
            {
                sb.AppendLine("  Middleware 3");
                await next();
            });

            var func = _pipelineBuilder.Build();
            await func.Invoke(_ctx, async () => { });

            //Just test! I know its stupid.
            var testInfo = sb.ToString();
            var str = RemoveWhiteSpace(testInfo);

            if ("13" != str)
            {
                Assert.Fail("Expected order: 1-3.\n" +
                            "Middleware execution order:\n" +
                            testInfo);
            }
            else
            {
                Assert.Pass();
            }

        }

        [Test]
        public async Task TestMiddlewareForceExit()
        {
            StringBuilder sb = new StringBuilder();
            _pipelineBuilder.Use(async (ctx, next) =>
            {
                sb.AppendLine("  Middleware 1");
                ctx.ForceExit();
                await next();
            });
            _pipelineBuilder.Use(async (ctx, next) =>
            {
                sb.AppendLine("  Middleware 2");
                await next();
            });
            _pipelineBuilder.Use(async (ctx, next) =>
            {
                sb.AppendLine("  Middleware 3");
                await next();
            });

            var func = _pipelineBuilder.Build();
            await func.Invoke(_ctx, async () => { });

            //Just test! I know its stupid.
            var testInfo = sb.ToString();
            var str = RemoveWhiteSpace(testInfo);

            if ("1" != str)
            {
                Assert.Fail("Expected order: 1.\n" +
                            "Middleware execution order:\n" +
                            testInfo);
            }
            else
            {
                Assert.Pass();
            }

        }

        [Test]
        public async Task TestMiddlewareDispose()
        {
            StringBuilder sb = new StringBuilder();
            _pipelineBuilder.Use(async (ctx, next) =>
            {
                sb.AppendLine("  Middleware 1");
                ctx.Dispose();
                await next();
            });
            _pipelineBuilder.Use(async (ctx, next) =>
            {
                sb.AppendLine("  Middleware 2");
                await next();
            });

            var func = _pipelineBuilder.Build();
            await func.Invoke(_ctx, async () => { });

            //Just test! I know its stupid.
            var testInfo = sb.ToString();
            var str = RemoveWhiteSpace(testInfo);

            if ("1" != str)
            {
                Assert.Fail("Expected order: 1.\n" +
                            "Middleware execution order:\n" +
                            testInfo);
            }
            else
            {
                Assert.Pass();
            }

        }

        string RemoveWhiteSpace(string testInfo)
        {
            var str = testInfo
                .Replace("Middleware", "")
                .Replace(" ", "")
                .Replace("\t", "")
                .Replace("\n", "")
                .Replace("\r", "");
            return str;
        }
    }
}