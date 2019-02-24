using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Extensions.DevExceptionMessage;
using Telegram.Bot.AspNetPipeline.Extensions.OldUpdatesIgnoring;
using Telegram.Bot.AspNetPipeline.Mvc.Builder;
using Telegram.Bot.AspNetPipeline.Mvc.Extensions.DebugExt;
using Telegram.Bot.AspNetPipeline.WebhookSupport;

namespace IRO.Samples.TelegramBotWithAsp
{
    //!Example how to use same service collection in asp.net and bot.
    public class Startup
    {
        /// <summary>
        /// Launch ngrok and copy domain string to test it.
        /// </summary>
        const string _domain = "https://fa4f04a3.ngrok.io";

        BotManager _botManager;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //Usage of same ServiceCollection for telegram library and asp.net is bad idea.
            //At first I thought differently, but in practice this model was not very convenient if you need to use many bots on one server process.
            //More precisely, the problem is the common services collection between the bots themselves.
            //or use separate collections for asp and bots.
            //But in this example i use one collection to show how you can do it, because it's really can be difficult.
            //And i recommend to add bot library after all services, to override some settings in service collection.
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var token = BotTokenResolver.GetToken();
            var bot = new Telegram.Bot.TelegramBotClient(token, new QueuedHttpClient(TimeSpan.FromSeconds(1)));
            _botManager = new BotManager(bot, services);

            //Invoked synchronous.
            _botManager.ConfigureServices((servicesWrap) =>
            {
                servicesWrap.AddMvc(new Telegram.Bot.AspNetPipeline.Mvc.Builder.MvcOptions()
                {
                    //Useful for debugging.
                    CheckEqualsRouteInfo = true
                });

                //Logging service example with NLog you can see in IRO.Tests.Telegram.
            });


        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.Use(async (ctx, next) =>
            {
                var path = ctx.Request.Path.ToString();
                if (path == "" || path == "/")
                    await ctx.Response.WriteAsync("Webhooks server. Please don't forget update domain constant.");
                await next();
            });

            //Telegram here.
            //Invoked on setup.
            _botManager.ConfigureBuilder(builder =>
            {
                builder.UseDevEceptionMessage();
                builder.UseOldUpdatesIgnoring();
                builder.UseMvc(mvcBuilder =>
                {
                    mvcBuilder.UseDebugInfo();
                });
            });

            //Note: update your pathTemplate and add there some string, that will identify telegram webhooks.
            //Something like: "wad5kK2PVL0SAEPq43q5cR2qwFWF4434/{0}". It must be same for all server processes.
            //=========
            //Use setWebhookAutomatically:false to configure how telegram webhook will work.
            var webhookReceiver = WebhookUpdatesReceiver.Create(
                app,
                _domain,
                pathTemplate: "telegram/update/{0}",
                setWebhookAutomatically: false
                );

            //!Use same service provider here.
            _botManager.Setup(app.ApplicationServices, webhookReceiver);

            //SetWebhookAsync not needed if setWebhookAutomatically is true.
            _botManager.BotContext.Bot.SetWebhookAsync(
                webhookReceiver.WebhookFullUrl,
                allowedUpdates: UpdateTypeExtensions.All
                ).Wait();
            _botManager.Start();
        }
    }
}
