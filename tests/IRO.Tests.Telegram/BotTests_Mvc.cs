using IRO.Tests.Telegram.Services;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Mvc.Builder;
using Telegram.Bot.CloudFileStorage;
using Telegram.Bot.CloudFileStorage.BotsProviders;
using Telegram.Bot.CloudFileStorage.Data;

namespace IRO.Tests.Telegram
{
    class BotTests_Mvc
    {
        public void Run(BotManager botManager, bool isDebug)
        {
            botManager.ConfigureServices((servicesWrap) =>
            {
                var serv = servicesWrap.Services;
                LoggerStarter.InitLogger(servicesWrap);
                servicesWrap.AddMvc(new MvcOptions()
                {
                    CheckEqualsRouteInfo = true
                });

                serv.AddSingleton<ITelegramBotClient>((sp) =>
                {
                    return botManager.BotContext.Bot;
                });
                serv.AddScoped<ISomeScopedService, SomeScopedService>();

                //Resource manager test
                serv.AddSingleton<ITelegramBotsProvider>((sp) =>
                {
                    return new OneTelegramBotProvider(botManager.BotContext.Bot);
                });
                serv.AddSingleton(new TgResourceManagerOptions()
                {
                    SaveResourcesChatId = BotTokenResolver.GetSaveResChatId()
                });
                serv.AddSingleton<TgResourceManager>();

                //Telegram storage test
                var opt = new TelegramStorageOptions()
                {
                    SaveResourcesChatId = BotTokenResolver.GetSaveResChatId(),
                    SaveOnSet = true
                };
                serv.AddSingleton(opt);
                serv.AddSingleton<TelegramStorage>();
            });

            botManager.ConfigureBuilder((builder) =>
            {
                if (isDebug)
                    builder.UseDevEceptionMessage();
                builder.UseExceptionHandler(async (ctx, ex) =>
                {
                    //Throw exception if false. False mean 'not handled'.
                    return false;
                });

                builder.UseMvc(mvcBuilder =>
                {
                    mvcBuilder.MapRouteAction(async (actionCtx) =>
                    {
                        await actionCtx.UpdateContext.SendTextMessageAsync("Mvc works.");
                    }, template: "/mvc");
                });
            });
            botManager.Start();
        }
    }
}
