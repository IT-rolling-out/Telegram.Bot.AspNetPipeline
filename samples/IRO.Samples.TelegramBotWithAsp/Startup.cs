using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Builder;

namespace IRO.Samples.TelegramBotWithAsp
{
    //!Example how to use same service collection in asp.net and bot.
    public class Startup
    {
        BotManager _botManager;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            
            var token = BotTokenResolver.GetToken();
            var bot = new Telegram.Bot.TelegramBotClient(token);
            _botManager = new BotManager(bot, services);
            _botManager.ConfigureServices((servWrapper) =>
            {
                //Invoked synchronous.
                servWrapper.AddMvc();
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseMvc();

            HostSetup.Setup("52860");

            _botManager.ConfigureBuilder(builder =>
            {
                //Invoked on setup.
                builder.UseMvc();
            });
            //!Use same service provider here.
            _botManager.Setup(app.ApplicationServices);
            _botManager.Start();
        }
    }
}
