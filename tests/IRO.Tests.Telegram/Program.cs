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
            
        }

        public static void CustomInit()
        {
            ITelegramBotClient bot = null;
            //Use asp.net collection, or empty. Use ioc which you want.
            IServiceCollection services = new ServiceCollection();

            var botHandler = new BotHandler(bot, services);
            botHandler.ConfigureBuilder((builder) =>
            {
                builder.Services.AddScoped<ISomeScopedService, SomeScopedService>();

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

                    //Custom controllers registration.
                    mvcBuilder.ConfigureControllers((controllersList) =>
                    {
                        controllersList.Add(typeof(BotFatherSampleController));
                    }, findWithReflection: false);

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
        }
    }
}
