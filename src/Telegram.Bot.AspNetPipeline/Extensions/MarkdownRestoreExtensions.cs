using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.AspNetPipeline.Extensions
{
    public static class MarkdownRestoreExtensions
    {
        public static string RestoreMarkdown(this Message msg)
        {
            if (msg.Entities==null || !msg.Entities.Any())
                return msg.Text;

            var text = msg.Text;
            var toInsert = new Dictionary<int, string>();

            foreach (var messageEntity in msg.Entities)
            {
                string markup = "";
                switch (messageEntity.Type)
                {
                    case MessageEntityType.Bold:
                        markup = "**";
                        break;
                    case MessageEntityType.Italic:
                        markup = "__";
                        break;
                    case MessageEntityType.Code:
                        markup = "`";
                        break;
                    case MessageEntityType.Pre:
                        markup = "```";
                        break;
                    default:
                        markup = "";
                        break;
                }

                var preStr = "";
                if (toInsert.TryGetValue(messageEntity.Offset, out var str))
                {
                    preStr = str;
                }
                toInsert[messageEntity.Offset] = preStr + markup;

                preStr = "";
                if (toInsert.TryGetValue(messageEntity.Offset + messageEntity.Length, out str))
                {
                    preStr = str;
                }
                toInsert[messageEntity.Offset + messageEntity.Length] = markup + preStr;
            }

            var stringBuilder = new StringBuilder("");
            for (var i=0;i<text.Length;i++)
            {
                if (toInsert.TryGetValue(i, out var markup))
                {
                    stringBuilder.Append(markup);
                }

                var c = text[i];
                stringBuilder.Append(c);
            }
            if (toInsert.TryGetValue(text.Length, out var markupLast))
            {
                stringBuilder.Append(markupLast);
            }
            return stringBuilder.ToString();
        }
    }
}
