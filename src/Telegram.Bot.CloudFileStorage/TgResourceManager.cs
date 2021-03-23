using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IRO.Common.Text;
using Newtonsoft.Json;
using Telegram.Bot.CloudFileStorage.BotsProviders;
using Telegram.Bot.CloudFileStorage.Consts;
using Telegram.Bot.CloudFileStorage.Data;
using Telegram.Bot.CloudFileStorage.Data.PostContentTypes;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using File = System.IO.File;

namespace Telegram.Bot.CloudFileStorage
{
    public class TgResourceManager
    {
        readonly ITelegramBotsProvider _telegramBotsProvider;
        readonly long _saveResChatId;

        public TgResourceManager(ITelegramBotsProvider telegramBotsProvider, TgResourceManagerOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));
            _saveResChatId = options.SaveResourcesChatId;
            _telegramBotsProvider = telegramBotsProvider;
        }

        /// <summary>
        /// ResourceFinalizer - will delete file after usage.
        /// </summary>
        /// <param name="resourceDto"></param>
        /// <returns></returns>
        public async Task<Stream> GetResourceAsStream(ResourceDto resourceDto)
        {
            var extType = resourceDto.ExtType;
            if (extType == PostResExtType.Sticker || extType == PostResExtType.Forward)
            {
                throw new Exception("Sticker and forward can't be downloaded.");
            }

            var sourceBot = await _telegramBotsProvider.GetBotClient(resourceDto.TgBotId);
            var fileId = resourceDto.TgFileId;
            var stream = new MemoryStream();
            await sourceBot.GetInfoAndDownloadFileAsync(fileId, stream);
            return stream;
        }

        #region Main.
        /// <summary>
        /// Save any file resource to telegram.
        /// </summary>
        public async Task<ResourceDto> UploadFileToTelegram(string resName, PostResExtType resType, string filePath, string caption = "")
        {
            using (var readStream = File.OpenRead(filePath))
            {
                return await UploadResourceStreamToTelegram(resName, resType, readStream, caption);
            }
        }

        /// <summary>
        /// Save any file resource to telegram.
        /// </summary>
        public async Task<ResourceDto> UploadResourceStreamToTelegram(string resName, PostResExtType resType, Stream readStream, string caption = "")
        {
            var botClient = await _telegramBotsProvider.GetRandomBotClient();
            switch (resType)
            {
                case PostResExtType.Audio:
                    {
                        var savedMsg = await botClient.SendAudioAsync(
                            _saveResChatId,
                            new InputOnlineFile(readStream, resName),
                            caption: caption
                        );
                        return new AudioResourceDto()
                        {
                            Name = resName,
                            TgFileId = savedMsg.Audio.FileId,
                            TgBotId = botClient.BotId
                        };
                    }

                case PostResExtType.Document:
                case PostResExtType.Unknown:
                    {
                        var savedMsg = await botClient.SendDocumentAsync(
                            _saveResChatId,
                            new InputOnlineFile(readStream, resName),
                            caption: caption
                        );
                        return new DocumentResourceDto
                        {
                            Name = resName,
                            TgFileId = savedMsg.Document.FileId,
                            TgBotId = botClient.BotId
                        };
                    }

                case PostResExtType.Voice:
                    {
                        var savedMsg = await botClient.SendVoiceAsync(
                            _saveResChatId,
                            new InputOnlineFile(readStream, resName)
                        );
                        return new VoiceResourceDto
                        {
                            Name = resName,
                            TgFileId = savedMsg.Voice.FileId,
                            TgBotId = botClient.BotId
                        };
                    }

                case PostResExtType.Image:
                    {
                        var savedMsg = await botClient.SendPhotoAsync(
                            _saveResChatId,
                            new InputOnlineFile(readStream, resName),
                            caption: caption
                        );

                        return new ImageResourceDto
                        {
                            Name = resName,
                            TgFileId = savedMsg.Photo.Last().FileId,
                            TgBotId = botClient.BotId
                        };
                    }
                case PostResExtType.Video:
                    {
                        var savedMsg = await botClient.SendVideoAsync(
                                _saveResChatId,
                                new InputOnlineFile(readStream, resName),
                                caption: caption
                            );

                        return new VideoResourceDto
                        {
                            Name = resName,
                            TgFileId = savedMsg.Video.FileId,
                            TgBotId = botClient.BotId
                        };
                    }
                case PostResExtType.Animation:
                    {
                        var savedMsg = await botClient.SendAnimationAsync(
                                _saveResChatId,
                                new InputOnlineFile(readStream, resName),
                                caption: caption
                            );

                        return new AnimationResourceDto
                        {
                            Name = resName,
                            TgFileId = savedMsg.Animation.FileId,
                            TgBotId = botClient.BotId
                        };
                    }
                case PostResExtType.VideoNote:
                    {
                        var savedMsg = await botClient.SendVideoNoteAsync(
                            _saveResChatId,
                            new InputOnlineFile(readStream, resName)
                        );
                        return new VideoNoteResourceDto
                        {
                            Name = resName,
                            TgFileId = savedMsg.VideoNote.FileId,
                            TgBotId = botClient.BotId
                        };
                    }
                default:
                    {
                        throw new Exception($"Resource type '{resType}' can't be saved from stream.");
                    }
            }
        }
        #endregion
    }
}
