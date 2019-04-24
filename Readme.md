# Telegram.Bot.AspNetPipeline
[![NuGet](https://img.shields.io/nuget/v/Telegram.Bot.AspNetPipeline.svg)](https://www.nuget.org/packages/Telegram.Bot.AspNetPipeline)

Telegram.Bot.AspNetPipeline is library, based on popular [Telegram.Bot](https://github.com/TelegramBots/telegram.bot), that allow developer to use many ASP.NET features to write telegram bots. And more, it also can make telegram bots development as simple as console applications development (see [ImprovedBot]()).

Which features added:
- Message processing pipeline based on same UpdateContext.
- IOC just like in ASP.NET. Using it you can replace almost any default service with yours implemention. 
- Controllers and routing with templates, order, UpdateType and custom routers.
- Model binding.
- Async requests processing.
- Many extensions to make development simpler.
- ReadMessageAsync extension based on chat context. See [ImprovedBot]().

## Quick start

You can use default initialization when start work with library.
All that you need - create any application and paste code below.

```csharp
        var bot = new TelegramBotClient("<token>");
        var botManager = new BotManager(bot);
        botManager.ConfigureServices((servicesWrap) =>
        {
            servicesWrap.AddMvc();
        });
        botManager.ConfigureBuilder(builder =>
        {
            builder.UseMvc();
        });
        botManager.Start();
```

Now it will support all main features (all that you can access in controolers).

Create controller class, inherit BotController.
Write some method and mark it with BotRouteAttribute.
NOTE: All used in routing methods must return Task.

```csharp
    public class BotFatherController : BotController
    {
        [BotRoute("/newbot", UpdateType.Message)]
        public async Task NewBot()
        {
        }
    }
```

Here you can see how would look telegram @BotFather command ```/newbot``` using Telegram.Bot.AspNetPipeline:

```csharp
    public class BotFatherController : BotController
    {
        [BotRoute("/newbot", UpdateType.Message)]
        public async Task NewBot()
        {
            await UpdateContext.SendTextMessageAsync("Enter bot name: ");
            var msg = await BotExt.ReadMessageAsync();
            var name = msg.Text;
            await Bot.SendTextMessageAsync(Chat.Id, "Enter bot nikname: ");
            msg = await BotExt.ReadMessageAsync();
            var nick = msg.Text;

            //Creating bot started...
            //If operation is too long, you can use CancellationToken to calcel it when cancellation requested, just like ReadMessageAsync do.
            UpdateProcessingAborted.ThrowIfCancellationRequested();
            //Creating bot finished...

            await Bot.SendTextMessageAsync(Chat.Id, "Bot created.");
        }
    }
```

Full code and more examples you can see in samples folder. I really recommend to do it, in quick start was shown small part of the functionality.

See also my other [libraries](https://github.com/IT-rolling-out).

## Contributing

[Here](https://github.com/IT-rolling-out/IRO#contributing) information about how to build and about codestyle.

## How it works?