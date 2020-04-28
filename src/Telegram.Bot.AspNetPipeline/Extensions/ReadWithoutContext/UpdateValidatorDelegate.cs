using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Extensions.ReadWithoutContext
{
    public delegate bool UpdateValidatorDelegate(ChatId chatId, Update upd);
}
