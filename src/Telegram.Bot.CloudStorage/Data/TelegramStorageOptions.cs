using System;
using System.IO;

namespace Telegram.Bot.CloudStorage.Data
{
    public class TelegramStorageOptions
    {
        public long SaveResourcesChatId { get; set; }

        /// <summary>
        /// Default is '{Path.GetTempPath()}/tg_one_file_storage.litedb'.
        /// </summary>
        public string FilePath { get; set; } = Path.Combine(Path.GetTempPath(), "tg_one_file_storage.json");

        /// <summary>
        /// Load storage file from telegeram on each Get call.
        /// <para></para>
        /// Will work really slow. Disabled by default. Useful when storage shared between processes.
        /// </summary>
        public bool LoadOnGet { get; set; }

        /// <summary>
        /// Upload storage file to telegeram on each Set call.
        /// <para></para>
        /// True if <see cref="LoadOnGet"/> is true.
        /// <para></para>
        /// Will work slow. Disabled by default. Useful when storage shared between processes.
        /// </summary>
        public bool SaveOnSet { get; set; }

        /// <summary>
        /// If null - autosave disabled. RequestSave will imediatly ForceSave to telegram. Default is 5 sec.
        /// <para></para>
        /// It save only if data is dirty.
        /// <para></para>
        /// Disabled if <see cref="SaveOnSet"/> or <see cref="LoadOnGet"/> is true.
        /// </summary>
        public TimeSpan? AutoSavesDelay { get; set; } = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Delete previous pinned message when save. False by default.
        /// </summary>
        public bool DeletePreviousMessages { get; set; }
    }
}