using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.AspNetPipeline.Extensions.KeyboardKit
{
    public class SimpleKeyboard : ISimpleKeyboard
    {
        readonly UpdateContext _updateContext;

        public string KeyboardTitleString { get; set; } = "⌨️.";

        public SimpleKeyboard(UpdateContext updateContext)
        {
            _updateContext = updateContext;
        }

        public async Task<Message> ShowInlineButtons(
            IEnumerable<string> buttons,
            string caption = null,
            bool twoInRow = false,
            bool deleteKeyboardMsgAfter = true
        )
        {
            var buttonsMultipleEnum = buttons.ConvertToMultiple(twoInRow);
            return await ShowInlineButtons(buttonsMultipleEnum, caption, deleteKeyboardMsgAfter);
        }

        public async Task<Message> ShowInlineButtons(
            IEnumerable<IEnumerable<string>> buttons,
            string caption = null,
            bool deleteKeyboardMsgAfter = true
        )
        {
            var buttonsList = new List<List<InlineKeyboardButton>> { };
            foreach (var buttonsLineStrList in buttons)
            {
                var buttonsLineList = new List<InlineKeyboardButton>();
                foreach (var str in buttonsLineStrList)
                {
                    var b = new KeyboardButton();
                    buttonsLineList.Add(str);
                }
                buttonsList.Add(buttonsLineList);
            }
            var replyMarkup = new InlineKeyboardMarkup(buttonsList);
            return await SendMessageWithButtons(replyMarkup, caption, deleteKeyboardMsgAfter);
        }

        public async Task<Message> ShowButtons(
            IEnumerable<string> buttons,
            string caption = null,
            bool twoInRow = false
            )
        {
            var buttonsMultipleEnum = buttons.ConvertToMultiple(twoInRow);
            return await ShowButtons(buttonsMultipleEnum, caption);
        }

        public async Task<Message> ShowButtons(
            IEnumerable<IEnumerable<string>> buttons,
            string caption = null,
            bool deleteKeyboardMsgAfter = true
        )
        {
            var buttonsList = new List<List<KeyboardButton>> { };
            foreach (var buttonsLineStrList in buttons)
            {
                var buttonsLineList = new List<KeyboardButton>();
                foreach (var str in buttonsLineStrList)
                {
                    buttonsLineList.Add(str);
                }
                buttonsList.Add(buttonsLineList);
            }
            var replyMarkup = new ReplyKeyboardMarkup(buttonsList, resizeKeyboard: true);

            return await SendMessageWithButtons(replyMarkup, caption, deleteKeyboardMsgAfter);
        }

        async Task<Message> SendMessageWithButtons(IReplyMarkup replyMarkup, string caption, bool deleteKeyboardMsgAfter)
        {
            var bot = _updateContext.Bot;
            var msg = await bot.SendTextMessageAsync(
                _updateContext.ChatId,
                caption ?? KeyboardTitleString,
                replyMarkup: replyMarkup
            );
            await DeleteLastKeyboardMessage();
            if (deleteKeyboardMsgAfter)
            {
                await SetLastKeyboardMessage(msg);
            }
            return msg;
        }

        async Task SetLastKeyboardMessage(Message msg)
        {
            await _updateContext.Session().Set("LastKeyboardButtonMsgId", msg.MessageId);
        }

        async Task DeleteLastKeyboardMessage()
        {
            try
            {
                var bot = _updateContext.Bot;
                var msgId = await _updateContext.Session().GetOrDefault<int?>("LastKeyboardButtonMsgId");
                if (msgId.HasValue)
                {
                    await bot.DeleteMessageAsync(_updateContext.ChatId, msgId.Value);
                    await _updateContext.Session().Set("LastKeyboardButtonMsgId", null);
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
        }

    }
}