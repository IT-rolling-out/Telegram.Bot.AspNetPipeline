using System;
using Microsoft.Extensions.Configuration;

namespace IRO.Samples.FileStorageWebApi
{
    public static class AppSettings
    {
        static IConfiguration Configuration { get; set; }

        public const string ApiPath = "api/"+ SwaggerApiVersion;

        public const string SwaggerApiName = "Files storage server example";

        public const string SwaggerApiVersion = "v1";

        public const int MaxFileSize = 52428800;

        public static string EXTERNAL_URL => Configuration["EXTERNAL_URL"];

        public static bool IS_DEBUG => Convert.ToBoolean(Configuration["IS_DEBUG"]);

        public static string TG_BOT_TOKEN => Configuration["TG_BOT_TOKEN"];

        public static long TG_SAVE_RESOURCES_CHAT => Convert.ToInt64(Configuration["TG_SAVE_RESOURCES_CHAT"]);

        public static void Init(IConfiguration configuration)
        {
            Configuration = configuration;
        }
    }
}