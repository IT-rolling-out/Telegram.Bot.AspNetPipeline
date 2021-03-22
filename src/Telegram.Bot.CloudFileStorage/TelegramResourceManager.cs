using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IRO.Common.Text;
using Newtonsoft.Json;
using Telegram.Bot.CloudFileStorage.Consts;
using Telegram.Bot.CloudFileStorage.Data;
using Telegram.Bot.CloudFileStorage.Data.PostContentTypes;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using File = System.IO.File;

namespace Telegram.Bot.CloudFileStorage
{
    public class TelegramResourceManager
    {
        string FilesBufDir { get; }

        readonly ITelegramBotsProvider _telegramBotsProvider;
        readonly long _saveResChatId;

        public TelegramResourceManager(ITelegramBotsProvider telegramBotsProvider, ResourceManagerInitOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            _saveResChatId = options.SaveResourcesChatId;
            _telegramBotsProvider = telegramBotsProvider;
            FilesBufDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "~files_buf");
            if (Directory.Exists(FilesBufDir))
            {
                Directory.Delete(FilesBufDir, true);
            }
            Directory.CreateDirectory(FilesBufDir);
        }

        /// <summary>
        /// ResourceFinalizer - will delete file after usage.
        /// </summary>
        /// <param name="resourceDto"></param>
        /// <returns></returns>
        public async Task<StreamWithFinalizer> GetResourceAsStream(ResourceDto resourceDto)
        {
            var filePath = await GetResourceAsFile(resourceDto);
            var readStream = File.OpenRead(filePath);
            return new StreamWithFinalizer(readStream, () => File.Delete(filePath));
        }

        /// <summary>
        /// Return file path.
        /// </summary>
        public async Task<string> GetResourceAsFile(ResourceDto resourceDto)
        {
            if (resourceDto.SourceType != PostResSourceType.TelegramPost)
            {
                throw new Exception("This method can be used only for TelegramPost SourceType.");
            }

            var extType = resourceDto.ExtType;
            if (extType == PostResExtType.Sticker || extType == PostResExtType.Forward)
            {
                throw new Exception("Sticker and forward can't be downloaded.");
            }

            var resName = resourceDto.Name ?? "file";
            var resourceBot = await _telegramBotsProvider.GetBotClient(resourceDto.TgBotId);
            var fileInfo = await SaveFileToTempDir(resourceBot,  resName, resourceDto.TgFileId);
            return fileInfo.FilePath;
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
                            TgBotId = botClient.BotId,
                            SourceType = PostResSourceType.TelegramPost
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
                            TgBotId = botClient.BotId,
                            SourceType = PostResSourceType.TelegramPost
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
                            TgBotId = botClient.BotId,
                            SourceType = PostResSourceType.TelegramPost
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
                            TgBotId = botClient.BotId,
                            SourceType = PostResSourceType.TelegramPost
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
                            TgBotId = botClient.BotId,
                            SourceType = PostResSourceType.TelegramPost
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
                            TgBotId = botClient.BotId,
                            SourceType = PostResSourceType.TelegramPost
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
                            TgBotId = botClient.BotId,
                            SourceType = PostResSourceType.TelegramPost
                        };
                    }
                default:
                    {
                        throw new Exception($"Resource type '{resType}' can't be saved from stream.");
                    }
            }
        }
        #endregion


        #region From telegram msg.
        public async Task<TelegramForwardResourceDto> SaveForwardFromMessage(ITelegramBotClient sourceBot, Message msg)
        {
            var resName = "forward";
            var savedMsg = await sourceBot.ForwardMessageAsync(_saveResChatId, msg.Chat.Id, msg.MessageId);

            return new TelegramForwardResourceDto
            {
                Name = resName,
                TgForwardChatId = savedMsg.Chat.Id,
                TgForwardMessageId = savedMsg.MessageId,
                TgBotId = sourceBot.BotId,
                SourceType = PostResSourceType.TelegramPost
            };
        }

        public async Task<TelegramStickerResourceDto> SaveStickerFromMessage(ITelegramBotClient sourceBot, Sticker sticker)
        {
            var resName = sticker.SetName + "_" + sticker.Emoji;
            return new TelegramStickerResourceDto
            {
                Name = resName,
                TgFileId = sticker.FileId,
                TgBotId = sourceBot.BotId,
                SourceType = PostResSourceType.TelegramPost
            };
        }

        public async Task<DocumentResourceDto> SaveDocumentFromMessage(ITelegramBotClient sourceBot, Document document)
        {
            var resName = document.FileName;
            var tempFileInfo = await SaveFileToTempDir(sourceBot, resName, document.FileId);

            var res = await UploadFileToTelegram(resName, PostResExtType.Document, tempFileInfo.FilePath, tempFileInfo.Caption);
            File.Delete(tempFileInfo.FilePath);
            return (DocumentResourceDto)res;
        }

        public async Task<VoiceResourceDto> SaveVoiceFromMessage(ITelegramBotClient sourceBot, Voice voice)
        {
            var resName = "voice.mp3";
            var tempFileInfo = await SaveFileToTempDir(sourceBot, resName, voice.FileId);

            var res = await UploadFileToTelegram(resName, PostResExtType.Voice, tempFileInfo.FilePath, tempFileInfo.Caption);
            File.Delete(tempFileInfo.FilePath);
            return (VoiceResourceDto)res;
        }

        public async Task<AudioResourceDto> SaveAudioFromMessage(ITelegramBotClient sourceBot, Audio audio)
        {
            var resName = "audio.mp3";
            var tempFileInfo = await SaveFileToTempDir(sourceBot, resName, audio.FileId);

            var res = await UploadFileToTelegram(resName, PostResExtType.Audio, tempFileInfo.FilePath, tempFileInfo.Caption);
            File.Delete(tempFileInfo.FilePath);
            return (AudioResourceDto)res;
        }

        public async Task<VideoNoteResourceDto> SaveVideoNoteFromMessage(ITelegramBotClient sourceBot, VideoNote videoNote)
        {
            var resName = "video_note.mp4";
            var tempFileInfo = await SaveFileToTempDir(sourceBot, resName, videoNote.FileId);

            var res = await UploadFileToTelegram(resName, PostResExtType.VideoNote, tempFileInfo.FilePath, tempFileInfo.Caption);
            File.Delete(tempFileInfo.FilePath);
            return (VideoNoteResourceDto)res;
        }

        public async Task<VideoResourceDto> SaveVideoFromMessage(ITelegramBotClient sourceBot, Video video)
        {
            var ext = video.MimeType.Substring(video.MimeType.IndexOf("/") + 1);
            var resName = "video." + ext;
            var tempFileInfo = await SaveFileToTempDir(sourceBot, resName, video.FileId);

            var res = await UploadFileToTelegram(resName, PostResExtType.Video, tempFileInfo.FilePath, tempFileInfo.Caption);
            File.Delete(tempFileInfo.FilePath);
            return (VideoResourceDto)res;
        }

        public async Task<AnimationResourceDto> SaveAnimationFromMessage(ITelegramBotClient sourceBot, Animation animation)
        {
            var resName = animation.FileName;
            var tempFileInfo = await SaveFileToTempDir(sourceBot, animation.FileName, animation.FileId);

            var res = await UploadFileToTelegram(resName, PostResExtType.Animation, tempFileInfo.FilePath, tempFileInfo.Caption);
            File.Delete(tempFileInfo.FilePath);
            return (AnimationResourceDto)res;
        }

        public async Task<ImageResourceDto> SaveImageFromMessage(ITelegramBotClient sourceBot, Message msg)
        {
            var resName = $"image_{TextExtensions.Generate(4)}.jpg";
            var photo = msg.Photo.Last();
            var tempFileInfo = await SaveFileToTempDir(sourceBot, resName, photo.FileId);

            var res = await UploadFileToTelegram(resName, PostResExtType.Image, tempFileInfo.FilePath, tempFileInfo.Caption);
            File.Delete(tempFileInfo.FilePath);
            return (ImageResourceDto)res;
        }
        #endregion

        async Task<(string Caption, string FilePath)> SaveFileToTempDir(ITelegramBotClient sourceBot, string resName, string fileId)
        {
            resName = TextExtensions.Generate(10) + "__" + resName;
            var filePath = Path.Combine(FilesBufDir, resName);
            using (var stream = File.OpenWrite(filePath))
            {
                await sourceBot.GetInfoAndDownloadFileAsync(fileId, stream);
            }

            var debugMsg = new Dictionary<string, object>()
            {
                {"resName", resName}
            };
            var caption = JsonConvert.SerializeObject(debugMsg, Formatting.Indented);
            return (Caption: caption, FilePath: filePath);
        }
    }
}
