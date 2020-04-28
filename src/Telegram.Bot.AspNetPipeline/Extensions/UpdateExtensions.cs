using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Extensions
{
    public static class UpdateExtensions
    {
        /// <summary>
        /// Return chat id for any update type. If chat can't be extracted from current update - return user id.
        /// </summary>
        public static ChatId ExtractChatId(this Update upd)
        {
            ChatId ch = null;
            if (upd.Message != null)
            {
                ch = upd.Message.Chat;
            }
            else if (upd.ChannelPost != null)
            {
                ch = upd.ChannelPost.Chat;
            }
            else if (upd.CallbackQuery != null)
            {
                ch = upd.CallbackQuery.Message.Chat;
            }
            else if (upd.InlineQuery != null)
            {
                ch = new ChatId(upd.InlineQuery.From.Id);
            }
            else if (upd.ChosenInlineResult != null)
            {
                ch = new ChatId(upd.ChosenInlineResult.From.Id);
            }
            else if (upd.PreCheckoutQuery != null)
            {
                ch = new ChatId(upd.PreCheckoutQuery.From.Id);
            }
            else if (upd.ShippingQuery != null)
            {
                ch = new ChatId(upd.ShippingQuery.From.Id);
            }
            else if (upd.EditedMessage != null)
            {
                ch = upd.EditedMessage.Chat;
            }
            else if (upd.EditedChannelPost != null)
            {
                ch = upd.EditedChannelPost.Chat;
            }
            else if (upd.PollAnswer != null)
            {
                ch = new ChatId(upd.PollAnswer.User.Id);
            }
            return ch;
        }

        /// <summary>
        /// Return from id for any update type.
        /// </summary>
        public static ChatId ExtractFromId(this Update upd)
        {
            ChatId ch = null;
            if (upd.Message != null)
            {
                ch = new ChatId(upd.Message.From.Id);
            }
            else if (upd.ChannelPost != null)
            {
                ch = new ChatId(upd.ChannelPost.From.Id);
            }
            else if (upd.CallbackQuery != null)
            {
                ch = new ChatId(upd.CallbackQuery.From.Id);
            }
            else if (upd.InlineQuery != null)
            {
                ch = new ChatId(upd.InlineQuery.From.Id);
            }
            else if (upd.ChosenInlineResult != null)
            {
                ch = new ChatId(upd.ChosenInlineResult.From.Id);
            }
            else if (upd.PreCheckoutQuery != null)
            {
                ch = new ChatId(upd.PreCheckoutQuery.From.Id);
            }
            else if (upd.ShippingQuery != null)
            {
                ch = new ChatId(upd.ShippingQuery.From.Id);
            }
            else if (upd.EditedMessage != null)
            {
                ch = new ChatId(upd.EditedMessage.From.Id);
            }
            else if (upd.EditedChannelPost != null)
            {
                ch = new ChatId(upd.EditedChannelPost.From.Id);
            }
            else if (upd.PollAnswer != null)
            {
                ch = new ChatId(upd.PollAnswer.User.Id);
            }
            return ch;
        }
    }
}
