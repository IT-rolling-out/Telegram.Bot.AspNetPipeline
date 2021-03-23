using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using IRO.Cache;
using IRO.Storage;
using IRO.Storage.DefaultStorages;
using NeoSmart.AsyncLock;
using Telegram.Bot.CloudFileStorage.Consts;
using Telegram.Bot.CloudFileStorage.Data;
using Telegram.Bot.CloudFileStorage.Data.PostContentTypes;

namespace Telegram.Bot.CloudFileStorage
{
    public class TelegramCloud
    {
        readonly TgResourceManager _resourceManager;
        readonly bool _useCache;
        readonly IKeyValueStorage _metadataStorage;
        readonly IKeyValueCache _cache;
        readonly AsyncLock _lock = new AsyncLock();

        public TelegramCloud(
            TelegramCloudStorageOptions opt,
            TgResourceManager resourceManager,
            IKeyValueStorage metadataStorage = null,
            IKeyValueCache cache = null
            )
        {
            _resourceManager = resourceManager ?? throw new ArgumentNullException(nameof(resourceManager));
            _metadataStorage = metadataStorage ?? new FileStorage();
            opt ??= new TelegramCloudStorageOptions();
            _useCache = opt.UseCache;
            _cache = cache ?? new FileSystemCache(100);

        }


        public async Task SaveFile(string key, Stream stream)
        {
            using (_lock.LockAsync())
            {
                var resourceDto = await _resourceManager.UploadResourceStreamToTelegram(
                    key,
                    PostResExtType.Document,
                    stream,
                    caption: key
                );
                await _metadataStorage.Set(key, resourceDto);
                if (_useCache)
                {
                    await _cache.Remove(key);
                }
            }
        }

        public async Task<Stream> LoadFile(string key)
        {
            using (_lock.LockAsync())
            {
                if (_useCache)
                {
                    try
                    {
                        return await _cache.GetStream(key);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Not found file in cache.", ex);
                    }
                }

                var resourceDto = await _metadataStorage.Get<ResourceDto>(key);
                return await _resourceManager.GetResourceAsStream(resourceDto);
            }
        }
    }
}
