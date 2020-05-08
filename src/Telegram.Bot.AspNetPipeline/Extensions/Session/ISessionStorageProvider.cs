using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Exceptions;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetPipeline.Extensions.Session
{
    public interface ISessionStorageProvider
    {
        ISessionStorage ResolveSessionStorage(long botId, long chatId);
    }
}
