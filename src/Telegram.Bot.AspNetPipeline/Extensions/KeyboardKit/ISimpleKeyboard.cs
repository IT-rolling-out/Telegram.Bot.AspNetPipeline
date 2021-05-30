using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Extensions.KeyboardKit
{
    public interface ISimpleKeyboard
    {
        string KeyboardTitleString { get; set; }

        Task<Message> ShowInlineButtons(
            IEnumerable<string> buttons,
            string caption = null,
            bool twoInRow = false,
            bool deleteKeyboardMsgAfter = true
        );

        Task<Message> ShowInlineButtons(
            IEnumerable<IEnumerable<string>> buttons,
            string caption = null,
            bool deleteKeyboardMsgAfter = true
        );

        Task<Message> ShowButtons(
            IEnumerable<string> buttons,
            string caption = null,
            bool twoInRow = false
        );

        Task<Message> ShowButtons(
            IEnumerable<IEnumerable<string>> buttons,
            string caption = null,
            bool deleteKeyboardMsgAfter = true
        );
    }
}