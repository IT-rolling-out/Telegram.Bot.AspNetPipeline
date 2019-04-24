using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Telegram.Bot.AspNetPipeline.Builder
{
    public delegate IServiceProvider ServiceProviderBuilderDelegate(ServiceCollection serviceCollection);
}
