using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Core
{
    public class UpdateReceivedEventArgs:EventArgs
    {
        public Update Update { get; }

        public UpdateReceivedEventArgs(Update update)
        {
            Update = update;
        }
    }
}
