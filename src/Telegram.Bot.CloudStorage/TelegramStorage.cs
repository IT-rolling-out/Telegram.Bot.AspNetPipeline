using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using IRO.Common.Services;
using IRO.Storage.DefaultStorages;
using NeoSmart.AsyncLock;
using Newtonsoft.Json;
using Telegram.Bot.CloudStorage.Data;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using File = System.IO.File;

namespace Telegram.Bot.CloudStorage
{
    public class TelegramStorage : BaseStorage
    {
        const string FileResName = "TelegramOneFileStorage.json";
        readonly ITelegramBotClient _botClient;
        readonly long _saveResChatId;
        readonly string _storageFilePath;
        readonly bool _loadOnGet;
        readonly bool _deletePreviousMessages;

        public TelegramStorage(TelegramStorageOptions opt, ITelegramBotClient botClient)
        {
            if (opt == null)
                throw new ArgumentNullException(nameof(opt));
            _botClient = botClient;
            _saveResChatId = opt.SaveResourcesChatId;
            _storageFilePath = opt.FilePath;
            _deletePreviousMessages = opt.DeletePreviousMessages;
            if (opt.LoadOnGet)
            {
                _loadOnGet = true;
                _saveOnSet = true;
            }
            else if (opt.SaveOnSet)
            {
                _saveOnSet = true;
            }
            else if (opt.AutoSavesDelay.HasValue)
            {
                _autoSaveEnabled = true;
                _autoSaveDelay = opt.AutoSavesDelay.Value;
            }
            ForceLoad().Wait();
        }

        #region telegram sync part.
        readonly AsyncLock _saveLock = new AsyncLock();
        readonly TimeSpan _autoSaveDelay;
        readonly bool _autoSaveEnabled;
        readonly bool _saveOnSet;
        bool _saveTaskStarted;

        /// <summary>
        /// Immediately save data.
        /// </summary>
        /// <returns></returns>
        public async Task ForceSave()
        {
            using (await _saveLock.LockAsync())
            {
                var chat = await _botClient.GetChatAsync(_saveResChatId);
                var prevPinnedMessage = chat.PinnedMessage;

                SaveStorageStateToFile();
                Message savedMsg = null;
                using (var readStream = File.OpenRead(_storageFilePath))
                {

                    savedMsg = await _botClient.SendDocumentAsync(
                        _saveResChatId,
                        new InputOnlineFile(readStream, FileResName),
                        caption: FileResName
                    );
                }
                await _botClient.PinChatMessageAsync(_saveResChatId, savedMsg.MessageId);
                if(_deletePreviousMessages)
                    await _botClient.DeleteMessageAsync(_saveResChatId, prevPinnedMessage.MessageId);
            }
        }

        /// <summary>
        /// Force load of storage file from telegram.
        /// Carefully, you can loose your unsaved data.
        /// </summary>
        public async Task ForceLoad()
        {
            using (await _saveLock.LockAsync())
            {
                var chat = await _botClient.GetChatAsync(_saveResChatId);
                if (chat.PinnedMessage?.Caption?.Trim() != FileResName)
                {
                    //Can't find storage.
                    File.WriteAllText(_storageFilePath, "{}");
                }
                else
                {
                    var fileId = chat.PinnedMessage.Document.FileId;
                    if (File.Exists(_storageFilePath))
                        File.Delete(_storageFilePath);
                    using (var stream = File.OpenWrite(_storageFilePath))
                    {
                        await _botClient.GetInfoAndDownloadFileAsync(fileId, stream);
                    }
                }

                LoadStorageStateFromFile();
            }
        }

        /// <summary>
        /// Save data with delay if dirty.
        /// </summary>
        protected async Task SyncedSave()
        {
            using (await _saveLock.LockAsync())
            {
                if (_saveTaskStarted)
                {
                    return;
                }

                if (!_autoSaveEnabled || _saveOnSet)
                {
                    //If SaveOnSet which enabled if LoadOnGet - will save immediately.
                    await ForceSave();
                    return;
                }

                //Delayed save task.s
                var bgTask = Task.Run(async () =>
                {
                    _saveTaskStarted = true;
                    try
                    {
                        await Task.Delay(_autoSaveDelay);
                        await ForceSave();
                    }
                    //catch (Exception ex)
                    //{
                    //    Debug.WriteLine("AutoSave failed.", ex);
                    //}
                    finally
                    {
                        _saveTaskStarted = false;
                    }
                });
            }
        }

        protected async Task SyncedLoad()
        {
            if (_loadOnGet)
            {
                await ForceLoad();
            }
        }
        #endregion

        #region File storage reworked part.
        const int FileAccessTimeoutSeconds = 30;
        IDictionary<string, string> _storageDict;

        protected override async Task InnerSet(string key, string value)
        {
            await SyncedLoad();
            _storageDict[key] = value;
            await SyncedSave();
        }

        protected override async Task InnerRemove(string key)
        {
            await SyncedLoad();
            _storageDict.Remove(key);
            await SyncedSave();
        }

        protected override async Task<string> InnerGet(string key)
        {
            await SyncedLoad();
            if (_storageDict.TryGetValue(key, out var value))
            {
                return value;
            }
            return null;
        }

        protected override async Task InnerClear()
        {
            _storageDict?.Clear();
            await SyncedSave();
        }
        
        protected void LoadStorageStateFromFile()
        {
            _storageDict = ReadStorage();
        }

        protected void SaveStorageStateToFile()
        {
            string serializedDict = JsonConvert.SerializeObject(_storageDict);
            WriteStorage(serializedDict);
        }

        Dictionary<string, string> ReadStorage()
        {
            Dictionary<string, string> res = null;
            try
            {
                FileHelpers.TryReadAllText(
                    _storageFilePath,
                    out string strFromFile,
                    FileAccessTimeoutSeconds
                    );
                res = JsonConvert.DeserializeObject<Dictionary<string, string>>(
                    strFromFile
                    );
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return res ?? new Dictionary<string, string>();
        }

        void WriteStorage(string storage)
        {
            if (!File.Exists(_storageFilePath))
            {
                File.CreateText(_storageFilePath).Close();
            }
            FileHelpers.TryWriteAllText(_storageFilePath, storage, FileAccessTimeoutSeconds);
        }
        #endregion
    }
}
