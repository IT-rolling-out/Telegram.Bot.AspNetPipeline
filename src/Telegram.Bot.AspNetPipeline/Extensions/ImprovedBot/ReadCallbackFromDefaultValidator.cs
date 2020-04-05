using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions.Logging;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot
{
    public static class ReadCallbackFromDefaultValidator
    {
        public static bool Check(UpdateContext newCtx, UpdateContext origCtx, ReadCallbackFromType fromType)
        {
            var upd = newCtx.Update;
            try
            {
                origCtx.Logger().LogTrace("Default CheckFromType for {0}.", fromType);
                if (fromType == ReadCallbackFromType.CurrentUser)
                {
                    return upd.Message.From.Id == origCtx.Message.From.Id;
                }
                else if (fromType == ReadCallbackFromType.CurrentUserReply)
                {
                    if (upd.Message.From.Id != origCtx.Message.From.Id)
                        return false;
                    if (upd.Message.ReplyToMessage?.From.Id != origCtx.Bot.BotId)
                        return false;
                    return true;
                }
                else if (fromType == ReadCallbackFromType.AnyUserReply)
                {
                    if (upd.Message.ReplyToMessage?.From.Id != origCtx.Bot.BotId)
                        return false;
                    return true;
                }
                else if (fromType == ReadCallbackFromType.AnyUser)
                {
                    //I know that current expression can be removed, but it make code more readable.
                    return true;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
