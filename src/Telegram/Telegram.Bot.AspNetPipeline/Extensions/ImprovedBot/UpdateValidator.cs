using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot
{
    public delegate Task<bool> UpdateValidator(Update update);
}
