using System;
using IRO.Mvc.CoolSwagger;
using IRO.Mvc.MvcExceptionHandler;
using IRO.Mvc.MvcExceptionHandler.Services;
using IRO.Samples.FileStorageWebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace FrameworkIRO.Utils
{
    public static class StartupInit
    {
        public static void UseExceptionBinder_Local(IApplicationBuilder app, bool isDebug)
        {
            app.UseMvcExceptionHandler((s) =>
            {
                s.IsDebug = isDebug;
                s.DefaultHttpCode = 500;
                s.CanBindByHttpCode = true;
                s.Host = AppSettings.EXTERNAL_URL;
                s.JsonSerializerSettings.Formatting = isDebug ? Formatting.Indented : Formatting.None;

                s.Mapping((builder) =>
                {
                    //Регистрируем исключение по http коду
                    builder.Register(
                        httpCode: 500,
                        errorKey: "InternalServerError"
                        );
                    builder.Register(
                        httpCode: 403,
                        errorKey: "Forbidden"
                        );
                    builder.Register(
                        httpCode: 400,
                        errorKey: "BadRequest"
                    );
                    builder.Register<UnauthorizedAccessException>(
                        httpCode: 401,
                        errorKey: "Unauthorized"
                    );
                    builder.RegisterAllAssignable<Exception>(
                        httpCode: 500,
                        errorKeyPrefix: ""
                        );
                });
            });
        }

        public static void UseSwaggerUI_Local(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.ShowExtensions();
                c.EnableValidator();
                c.SwaggerEndpoint($"/swagger/{AppSettings.SwaggerApiVersion}/swagger.json", AppSettings.SwaggerApiName);
                c.DisplayOperationId();
                c.DisplayRequestDuration();
            });


        }
        public static void AddSwaggerGen_Local(IServiceCollection services)
        {

            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc(
                    AppSettings.SwaggerApiVersion,
                    new OpenApiInfo
                    {
                        Title = AppSettings.SwaggerApiName,
                        Version = AppSettings.SwaggerApiVersion,
                        Description = "Api description"
                    });
                opt.EnableAnnotations();
                opt.UseCoolSummaryGen();
                opt.UseDefaultIdentityAuthScheme();
                opt.AddSwaggerTagNameOperationFilter();
                opt.AddDefaultResponses(new ResponseDescription()
                {
                    StatusCode = 500,
                    Description = "Server visible error."
                });
            });
        }
    }
}
