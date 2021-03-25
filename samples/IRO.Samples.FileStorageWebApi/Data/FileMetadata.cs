using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace IRO.Samples.FileStorageWebApi.Data
{
    public class FileMetadata
    {
        public string ContentDisposition { get; set; }

        public string ContentType { get; set; }

        public string FileName { get; set; }

        public IDictionary<string, StringValues> Headers { get; set; }

        public long Length { get; set; }

        public string Name { get; set; }

        public DateTimeOffset LastModified { get; set; }

        public static FileMetadata FromFormFile(IFormFile formFile)
        {
            var obj = new FileMetadata()
            {
                FileName = formFile.FileName,
                ContentType = formFile.ContentType,
                ContentDisposition = formFile.ContentDisposition,
                Headers = formFile.Headers,
                Length = formFile.Length,
                Name = formFile.FileName,
                LastModified=DateTimeOffset.UtcNow
            };
            return obj;
        }
    }
}
