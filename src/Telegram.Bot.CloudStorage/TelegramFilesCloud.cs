using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using IRO.Cache;
using IRO.Common.Text;
using IRO.Storage;
using IRO.Storage.DefaultStorages;
using NeoSmart.AsyncLock;
using Telegram.Bot.CloudStorage.Data;
using Telegram.Bot.Types.InputFiles;

namespace Telegram.Bot.CloudStorage
{
    public class TelegramFilesCloud : TelegramFilesCloud<object>
    {
        public TelegramFilesCloud(ITelegramBotClient botClient, TelegramFilesCloudOptions opt, IKeyValueStorage metadataStorage = null, IKeyValueCache cache = null) : base(botClient, opt, metadataStorage, cache)
        {
        }
    }

    public class TelegramFilesCloud<TMetadata>
        where TMetadata : class
    {
        readonly ITelegramBotClient _botClient;
        readonly bool _useCache;
        readonly IKeyValueStorage _metadataStorage;
        readonly IKeyValueCache _cache;
        readonly AsyncLock _lock = new AsyncLock();
        readonly bool _deleteOlderFiles;
        readonly long _saveResChatId;

        public TelegramFilesCloud(
            ITelegramBotClient botClient,
            TelegramFilesCloudOptions opt,
            IKeyValueStorage metadataStorage = null,
            IKeyValueCache cache = null
            )
        {
            _botClient = botClient;
            _metadataStorage = metadataStorage ?? new FileStorage();
            opt ??= new TelegramFilesCloudOptions();
            _useCache = opt.UseCache;
            _saveResChatId = opt.SaveResourcesChatId;
            _deleteOlderFiles = opt.DeleteOlderFiles;
            _cache = cache ?? new FileSystemCache(100);

        }

        public async Task DeleteFile(string key)
        {
            key = EscapeKey(key);
            using (await _lock.LockAsync())
            {
                var metadata = await _metadataStorage.GetOrDefault<TgFileMetadata<TMetadata>>(key);
                if (metadata == null)
                {
                    return;
                }

                await _metadataStorage.Remove(key);
                await _botClient.DeleteMessageAsync(_saveResChatId, metadata.MessageId);

                if (_useCache)
                {
                    await _cache.Remove(key);
                }
            }
        }


        public async Task SaveFile(string key, Stream stream, TMetadata yourMetadata = null)
        {
            key = EscapeKey(key);
            using (await _lock.LockAsync())
            {
                if (_deleteOlderFiles)
                    await DeleteFile(key);

                var fileName = TextExtensions.Generate(10);
                var savedMsg = await _botClient.SendDocumentAsync(
                        _saveResChatId,
                        new InputOnlineFile(stream, fileName),
                        caption: fileName
                    );

                var metadata = new TgFileMetadata<TMetadata>()
                {
                    FileId = savedMsg.Document.FileId,
                    MessageId = savedMsg.MessageId,
                    UserCastomMetadata = yourMetadata
                };
                await _metadataStorage.Set(key, metadata);
                if (_useCache)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    await _cache.SetStream(key, stream);
                }
            }
        }

        public async Task<TMetadata> GetFileMetadata(string key)
        {
            key = EscapeKey(key);
            using (await _lock.LockAsync())
            {
                var metadata = await _metadataStorage.GetOrDefault<TgFileMetadata<TMetadata>>(key);
                if (metadata == null)
                {
                    throw new Exception("File not found.");
                }

                return metadata.UserCastomMetadata;
            }
        }

        public async Task<Stream> LoadFile(string key)
        {
            key = EscapeKey(key);
            using (await _lock.LockAsync())
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

                var metadata = await _metadataStorage.GetOrDefault<TgFileMetadata<TMetadata>>(key);
                if (metadata == null)
                {
                    throw new Exception("File not found.");
                }


                if (_useCache)
                {
                    using (var stream = new MemoryStream())
                    {
                        await _botClient.GetInfoAndDownloadFileAsync(metadata.FileId, stream);
                        await _cache.SetStream(key, stream);
                        return await _cache.GetStream(key);
                    }
                }
                else
                {
                    var stream = new MemoryStream();
                    await _botClient.GetInfoAndDownloadFileAsync(metadata.FileId, stream);
                    return stream;
                }
            }
        }

        string EscapeKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new Exception("Key is invalid.");
            }
            //Used to prevent scopes usage in metadata storage.
            return key.Replace(".", "_DOT_");
        }
    }
}
