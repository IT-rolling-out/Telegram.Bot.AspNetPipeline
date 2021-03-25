using IRO.Storage;
using IRO.Tests.Telegram.Services;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Mvc.Builder;
using Telegram.Bot.CloudStorage;
using Telegram.Bot.CloudStorage.Data;

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


                //Telegram storage test
                var opt = new TelegramStorageOptions()
                {
                    SaveResourcesChatId = BotTokenResolver.GetSaveResChatId(),
                    SaveOnSet = true
                };
                serv.AddSingleton(opt);
                serv.AddSingleton<IKeyValueStorage, TelegramStorage>();
                serv.AddSingleton<TelegramFilesCloud>();
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
