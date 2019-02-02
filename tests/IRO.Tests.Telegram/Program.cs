using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using IRO.Tests.Telegram.Controllers;
using IRO.Tests.Telegram.Services;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.AspNetPipeline.Builder;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions.DevExceptionMessage;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot;
using Telegram.Bot.AspNetPipeline.Mvc.Builder;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace IRO.Tests.Telegram
{
    class Program
    {
        static void Main(string[] args)
        {
            var token = BotStaticTestsHelpers.GetToken();
            var bot = new TelegramBotClient(token);
            var botHandler = new BotHandler(bot);
            TestReadMiddleware(botHandler);
            while (true)
            {
                Console.ReadLine();
            }
        }

        public static void TestReadMiddleware(BotHandler botHandler)
        {
            botHandler.ConfigureServices((services) =>
            {

            });

            botHandler.ConfigureBuilder((builder) =>
            {
                builder.UseDevEceptionMessage();
                builder.Use(async (ctx, next) =>
                {
                    if (ctx.Message?.Text==null)
                    {
                        await ctx.SendTextMessageAsync("Not text message.");
                        ctx.ForceExit();
                        return;
                    }

                    var ctxTrimmedText = ctx.Message.Text.Trim();
                    Message msg = null;
                    if (ctxTrimmedText.StartsWith("/help"))
                    {
                        await ctx.SendTextMessageAsync("Commands:\n" +
                                                       "/current_user_reply\n" +
                                                       "/current_user\n" +
                                                       "/any_user\n" +
                                                       "/any_user_reply");
                        ctx.Processed();
                    }
                    else if (ctxTrimmedText.StartsWith("/any_user_reply"))
                    {
                        await ctx.SendTextMessageAsync("Reply to bot to process message.");
                        msg = await ctx.BotExt.ReadMessageAsync(ReadCallbackFromType.AnyUserReply);
                    }
                    else if (ctxTrimmedText.StartsWith("/current_user_reply"))
                    {
                        await ctx.SendTextMessageAsync("Reply to bot to process message.");
                        msg = await ctx.BotExt.ReadMessageAsync(ReadCallbackFromType.CurrentUserReply);
                    }
                    else if (ctxTrimmedText.StartsWith("/current_user"))
                    {
                        msg = await ctx.BotExt.ReadMessageAsync();
                    }
                    else if (ctxTrimmedText.StartsWith("/any_user"))
                    {
                        msg = await ctx.BotExt.ReadMessageAsync(ReadCallbackFromType.AnyUser);
                    }

                    if (msg != null)
                    {
                        ctx.Processed();
                        var msgText = msg.Text;
                        await ctx.SendTextMessageAsync($"Command : '{ctxTrimmedText}'.\n" +
                                                       $"Awaited msg: '{msgText}'.");
                    }

                    
                    await next();
                });
                builder.Use(async (ctx, next) =>
                {
                    //if (!ctx.IsProcessed)
                    //    await ctx.SendTextMessageAsync($"Not processed '{ctx.Message.Text}'.");
                    await next();
                });
            });
            botHandler.Start();
        }

        public static void CustomInit()
        {
            ITelegramBotClient bot = null;
            //Use asp.net collection, or empty. Use ioc which you want.
            IServiceCollection serviceCollection = new ServiceCollection();

            var botHandler = new BotHandler(bot, serviceCollection);

            botHandler.ConfigureServices((services) =>
            {
                services.AddScoped<ISomeScopedService, SomeScopedService>();
                services.AddMvc((mvcServicesRegistrator) =>
                {
                    //Custom controllers registration.
                    mvcServicesRegistrator.ConfigureControllers((controllersList) =>
                    {
                        controllersList.Add(typeof(BotFatherSampleController));
                    }, findWithReflection: false);
                });
            });

            botHandler.ConfigureBuilder((builder) =>
            {
                builder.UseMvc((mvcBuilder) =>
                {
                    //Mvc route example. Can configure just like default controller.
                    mvcBuilder.MapRouteAction(
                        async (actionCtx) =>
                        {
                            await actionCtx.UpdateContext.Bot.SendTextMessageAsync(
                                actionCtx.UpdateContext.Chat,
                                "Hi!"
                                );

                        }, template: "/hi", order: 1);


                    //NOTE. Services from UseMvc not registered in IOC, when all services from AddMvc is registered.
                    //mvcBuilder.ControllerActionPreparer=...
                    //mvcBuilder.Routers=...
                    //mvcBuilder.ControllersFactory=...
                });

                //Middleware example.
                builder.Use(async (ctx, next) =>
                {
                    if (!ctx.IsProcessed)
                    {
                        await ctx.Bot.SendTextMessageAsync(
                                ctx.Chat,
                                "Enter /help ."
                                );
                    }
                    await next();
                });
            });
        }

        public static void DefaultInit()
        {
            ITelegramBotClient bot = null;

            var botHandler = new BotHandler(bot);
            botHandler.ConfigureBuilder((builder) =>
            {
                builder.UseMvc();
            });

            botHandler.ConfigureServices((services) =>
            {
                services.AddMvc();
            });
        }
    }
}
