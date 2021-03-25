using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FrameworkIRO.Utils;
using IRO.Mvc.Core;
using IRO.Mvc.MvcExceptionHandler;
using IRO.Samples.FileStorageWebApi.Data;
using IRO.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.CloudStorage;
using Telegram.Bot.CloudStorage.Data;

namespace IRO.Samples.FileStorageWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AppSettings.Init(configuration);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Increase file upload size.
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = AppSettings.MaxFileSize;
                x.MultipartBodyLengthLimit = AppSettings.MaxFileSize; // In case of multipart
            });

            services.AddControllers();
            services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));
            StartupInit.AddSwaggerGen_Local(services);
            services.AddMvcExceptionHandler();

            //Telegram bot part.
            var bot = new TelegramBotClient(
                AppSettings.TG_BOT_TOKEN,
                new QueuedHttpClient(TimeSpan.FromMilliseconds(50))
            );
            services.AddSingleton<ITelegramBotClient>(bot);

            //Telegram storage part.
            var opt = new TelegramStorageOptions()
            {
                SaveResourcesChatId = AppSettings.TG_SAVE_RESOURCES_CHAT
            };
            services.AddSingleton(opt);
            services.AddSingleton<IKeyValueStorage, TelegramStorage>();
            //Telegram files limit is 50 mb.
            //Note that with CacheAndNotWait you can operate with any files, but too big files will be not saved in telegram,
            //so they will be unavailable after app restart.
            services.AddSingleton(new TelegramFilesCloudOptions()
            {
                SaveResourcesChatId = AppSettings.TG_SAVE_RESOURCES_CHAT,
                CacheAndNotWait = true,
                DeleteOlderFiles = false
            });
            services.AddSingleton<TelegramFilesCloud<FileMetadata>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Increase file upload size.
            app.UseWhen(context => context.Request.Path.StartsWithSegments($"/{AppSettings.ApiPath}/files/upload"),
                appBuilder =>
                {
                    appBuilder.Use(async (ctx, next) =>
                    {
                        ctx.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = AppSettings.MaxFileSize;
                        await next();
                    });
                });

            app.UseCors("CorsPolicy");
            if (AppSettings.IS_DEBUG)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            StartupInit.UseExceptionBinder_Local(app, AppSettings.IS_DEBUG);
            StartupInit.UseSwaggerUI_Local(app);

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(env.ContentRootPath, "FrontApp")
                    ),
                RequestPath = ""
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }

    }

}
