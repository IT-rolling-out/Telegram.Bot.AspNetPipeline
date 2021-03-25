using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using IRO.Samples.FileStorageWebApi.Data;
using IRO.Storage;
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
        const string UploadsLogCacheKey = "UploadsLogCacheKey";

        readonly IKeyValueStorage _storage;
        readonly TelegramFilesCloud<FileMetadata> _filesCloud;

        public FileUploadController(IKeyValueStorage storage, TelegramFilesCloud<FileMetadata> filesCloud)
        {
            _storage = storage;
            _filesCloud = filesCloud;
        }

        [HttpGet("uploadLogs")]
        public async Task<IEnumerable<UploadLogRecord>> GetUploadLogs()
        {
            return await GetLogRecordsList();
        }

        [HttpPost("upload")]
        public async Task<UploadLogRecord> UploadFile(IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                var metadata = FileMetadata.FromFormFile(uploadedFile);
                using (var fileStream = uploadedFile.OpenReadStream())
                {
                    await _filesCloud.SaveFile(uploadedFile.FileName, fileStream, metadata);
                }
                return await AddLogRecord(uploadedFile.FileName);
            }
            else
            {
                throw new Exception("File not uploaded.");
            }
        }

        [HttpGet("download/{fileName}")]
        public async Task<FileStreamResult> DownloadFile([FromRoute] string fileName)
        {
            var metadata = await _filesCloud.GetFileMetadata(fileName);
            var fileStream = await _filesCloud.LoadFile(fileName);
            return File(
                fileStream,
                metadata.ContentType,
                metadata.FileName,
                metadata.LastModified,
                null
                );
        }

        async Task<UploadLogRecord> AddLogRecord(string fileName)
        {
            var urlFileName = HttpUtility.UrlEncode(fileName);
            var record = new UploadLogRecord()
            {
                UploadedAt = DateTime.UtcNow,
                FileName = fileName,
                DownloadUrl = $"{ AppSettings.EXTERNAL_URL}/{AppSettings.ApiPath}/files/download/{urlFileName}"
            };
            var list =await GetLogRecordsList();
            list.Add(record);
            await _storage.Set(UploadsLogCacheKey, list);
            return record;
        }

        async Task<List<UploadLogRecord>> GetLogRecordsList()
        {
            return await _storage.GetOrDefault<List<UploadLogRecord>>(UploadsLogCacheKey) ?? new List<UploadLogRecord>();
            
        }
    }
}
