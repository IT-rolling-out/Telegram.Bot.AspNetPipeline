namespace Telegram.Bot.CloudStorage.Data
{
    public class TelegramFilesCloudOptions
    {
        public bool UseCache { get; set; } = true;

        public long SaveResourcesChatId { get; set; }

        /// <summary>
        /// Delete old file with same key as new file. True by default.
        /// </summary>
        public bool DeleteOlderFiles { get; set; } = true;
    }
}