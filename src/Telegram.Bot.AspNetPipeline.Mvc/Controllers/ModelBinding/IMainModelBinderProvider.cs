using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.ModelBinding.Binders;
using Telegram.Bot.AspNetPipeline.Mvc.Extensions.Main;

namespace Telegram.Bot.AspNetPipeline.Mvc.Controllers.ModelBinding
{
    /// <summary>
    /// Implemented with <see cref="ServicesBus"/>.
    /// </summary>
    public interface IMainModelBinderProvider
    {
        MainModelBinder MainModelBinder { get; }
    }
}
