using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.AspNetPipeline.Core
{
    public static class UpdateTypeExtensions
    {
        public static UpdateType[] All => new UpdateType[]
        {
            UpdateType.CallbackQuery,
            UpdateType.ChannelPost,
            UpdateType.ChosenInlineResult,
            UpdateType.EditedChannelPost,
            UpdateType.EditedMessage,
            UpdateType.InlineQuery,
            UpdateType.Message,
            UpdateType.PreCheckoutQuery,
            UpdateType.ShippingQuery,
            UpdateType.Unknown
        };
    }
}
