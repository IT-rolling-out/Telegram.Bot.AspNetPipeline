using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IRO.Samples.FileStorageWebApi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Telegram.Bot.CloudStorage;

namespace IRO.Samples.FileStorageWebApi.Controllers
{
    [ApiController]
    [Route(AppSettings.ApiPath + "/files")]
    public class FileUploadController : ControllerBase
    {
        TelegramFilesCloud<FileMetadata> _tgCloud;

        public FileUploadController(TelegramFilesCloud<FileMetadata> tgCloud)
        {
            _tgCloud = tgCloud;
        }

        [HttpPost("upload/{fileName}")]
        public async Task<JsonResult> UploadFile(IFormFile uploadedFile, [FromRoute] string fileName)
        {
            if (uploadedFile != null)
            {
                var metadata = FileMetadata.FromFormFile(uploadedFile);
                using (var fileStream = uploadedFile.OpenReadStream())
                {
                    await _tgCloud.SaveFile(fileName, fileStream, metadata);
                }
            }
            else
            {
                throw new Exception("File not uploaded.");
            }
            return new JsonResult("File uploaded.");
        }

        [HttpGet("download/{fileName}")]
        public async Task<FileStreamResult> DownloadFile([FromRoute] string fileName)
        {
            var metadata = await _tgCloud.GetFileMetadata(fileName);
            var fileStream = await _tgCloud.LoadFile(fileName);
            return File(
                fileStream,
                metadata.ContentType,
                metadata.FileName,
                metadata.LastModified,
                null
                );
        }
    }
}
