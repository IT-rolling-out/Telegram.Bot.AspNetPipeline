namespace Telegram.Bot.CloudStorage.Data
{
    public class TelegramFilesCloudOptions
    {
        /// <summary>
        /// Default is true.
        /// </summary>
        public bool UseCache { get; set; } = true;

        /// <summary>
        /// Send file to telegram asynchronously. Default is false. Can work only if cache enabled.
        /// <para></para>
        /// It works extremely fast, because app only work with cached data, but you can't track sended files to telegram successfully or not.
        /// <para></para>
        /// Use it when you have many write|read operations with not important files. Recommend to set <see cref="DeleteOlderFiles"/> to false.
        /// </summary>
        public bool CacheAndNotWait { get; set; } = false;

        public long SaveResourcesChatId { get; set; }

        /// <summary>
        /// Delete old file with same key as new file. True by default.
        /// </summary>
        public bool DeleteOlderFiles { get; set; } = true;
    }
}