namespace Telegram.Bot.AspNetPipeline.Core.Services
{
    public class PollingUpdatesReceiver : IUpdatesReceiver
    {
        private BotManager _botManager;
        private ITelegramBotClient _bot;

        public bool IsReceiving => _bot.IsReceiving;

        public event UpdateReceivedEvent UpdateReceived;

        public void StartReceiving()
        {
            _bot.SetWebhookAsync("").Wait();
            _bot.StartReceiving();
            _bot.OnUpdate += Handler;
        }

        public void StopReceiving()
        {
            _bot.StopReceiving();
            _bot.OnUpdate -= Handler;
        }

        public void Init(BotManager botManager)
        {
            _botManager = botManager;
            _bot = _botManager.BotContext.Bot;
        }

        public void BotManagerDisposed()
        {
            _bot.OnUpdate -= Handler;
            _bot = null;
            _botManager = null;
        }

        private void Handler(object sender, UpdateEventArgs args)
        {
            var passedArgs = new UpdateReceivedEventArgs(args.Update);
            UpdateReceived?.Invoke(sender, passedArgs);
        }
    }

}
