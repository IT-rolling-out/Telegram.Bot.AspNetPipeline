using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.AspNetPipeline.Core
{
    class UpdateContextSearchBag
    {
        ConcurrentDictionary<string, UpdateContext> _dict = new ConcurrentDictionary<string, UpdateContext>();

        public IList<UpdateContext> Find(
            ChatId ChatId, 
            int? BotId=null, 
            IEnumerable<UpdateType> UpdateTypes=null
            )
        {

        }

        public static string CreateDescriptor(UpdateContext updateContext, IEnumerable<UpdateType> allowedUpdateTypes)
        {
            var sd = new UpdateContextSearchData();
            sd.CurrentUpdateContext = updateContext;
            sd.ChatId = updateContext.Update.Message?.Chat.Id;
            sd.ChatId = updateContext.ChatId;
        }



    }

    class UpdateContextSearchData
    {
        public ChatId ChatId { get; set; }

        public int? BotId { get; set; }

        public UpdateContext CurrentUpdateContext { get; set; }
    }
}
