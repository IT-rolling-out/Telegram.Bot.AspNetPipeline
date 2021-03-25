namespace Telegram.Bot.CloudStorage.Data
{
    class TgFileMetadata<TMetadata>
        where TMetadata : class
    {
        public int MessageId { get; set; }

        public string FileId { get; set; }

        public TMetadata UserCastomMetadata { get; set; }
    }
}
