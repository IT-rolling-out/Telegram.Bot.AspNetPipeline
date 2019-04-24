using System;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot.UpdateContextFastSearching
{
    /// <summary>
    /// Specialized only for "ImprovedBot".
    /// </summary>
    public struct UpdateContextSearchData
    {
        public long ChatId { get; set; }

        public int BotId { get; set; }

        public UpdateContext CurrentUpdateContext { get; set; }

        public TaskCompletionSource<Update> TaskCompletionSource { get; set; }

        public UpdateValidatorDelegate UpdateValidator { get; set; }

        public string Key => CreateKey(ChatId, BotId);

        public static string CreateKey(long chatId, int botId)
        {
            return chatId.ToString() + botId.ToString();
        }
    }

}
