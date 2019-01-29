using System;
using System.Threading;
using System.Threading.Tasks;
using IRO.Tests.Telegram.Controllers;
using IRO.Tests.Telegram.Services;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.AspNetPipeline.Core.Builder;
using Telegram.Bot.AspNetPipeline.Mvc.Core.Builder;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace IRO.Tests.Telegram
{
    class Program
    {
        static void Main(string[] args)
        {
            TelegramBotClient bot=null;
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
