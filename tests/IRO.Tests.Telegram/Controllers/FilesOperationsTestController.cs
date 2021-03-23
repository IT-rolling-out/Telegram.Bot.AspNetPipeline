using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using IRO.Common.Services;
using IRO.Tests.Telegram.Services;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Metadata;
using Telegram.Bot.CloudFileStorage;
using Telegram.Bot.CloudFileStorage.Consts;

namespace IRO.Tests.Telegram.Controllers
{
    public class FilesOperationsTestController : BotController
    {
        readonly TgResourceManager _telegramResourceManager;

        public FilesOperationsTestController(TgResourceManager telegramResourceManager)
        {
            _telegramResourceManager = telegramResourceManager;
        }

        [BotRoute("/files_cloud_test")]
        public async Task FileStorageTest()
        {
            var resourceDto = await _telegramResourceManager.UploadFileToTelegram(
                "test.jpg",
                PostResExtType.Document,
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.jpg")
            );
            await SendTextMessageAsync($"Test.");
        }

    }
}