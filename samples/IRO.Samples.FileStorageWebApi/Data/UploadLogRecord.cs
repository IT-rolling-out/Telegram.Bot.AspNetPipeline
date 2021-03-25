using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IRO.Samples.FileStorageWebApi.Data
{
    public class UploadLogRecord
    {
        public DateTime UploadedAt { get; set; }

        public string FileName { get; set; }

        public string DownloadUrl { get; set; }
    }
}
