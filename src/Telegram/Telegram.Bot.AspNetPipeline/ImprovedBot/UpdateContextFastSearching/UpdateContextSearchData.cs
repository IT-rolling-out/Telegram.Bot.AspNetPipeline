using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Core
{
    /// <summary>
    /// Specialized only for "ImprovedBot".
    /// </summary>
    public struct UpdateContextSearchData
    {
        public long ChatId { get; set; }

        public int BotId { get; set; }

        public UpdateContext CurrentUpdateContext { get; set; }

        public TaskCompletionSource<Message> TaskCompletionSource { get; set; }

        public Func<Update, bool> MessageValidator { get; set; }

        public string Key => CreateKey(ChatId, BotId);

        public static string CreateKey(long chatId, int botId)
        {
            return chatId.ToString() + botId.ToString();
        }
    }

}
