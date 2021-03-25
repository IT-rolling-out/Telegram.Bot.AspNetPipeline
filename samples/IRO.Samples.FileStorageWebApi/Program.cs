using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IRO.Samples.FileStorageWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    try
                    {
                        var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                        if (string.IsNullOrWhiteSpace(envName))
                        {
                            config.AddJsonFile("appsettings.json");
                        }
                        else
                        {
                            config.AddJsonFile($"appsettings.{envName}.json");
                        }
                    }
                    catch
                    {
#if DEBUG
                        throw;
#endif
                    }
                    //Override by external.
                    config.AddEnvironmentVariables();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
