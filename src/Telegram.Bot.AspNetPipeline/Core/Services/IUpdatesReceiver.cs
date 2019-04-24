namespace Telegram.Bot.AspNetPipeline.Core.Services
{
    /// <summary>
    /// Always use one receiver for one <see cref="BotManager"/>.
    /// </summary>
    public interface IUpdatesReceiver
    {
        bool IsReceiving { get; }

        event UpdateReceivedEvent UpdateReceived;

        void StartReceiving();

        void StopReceiving();

        /// <summary>
        /// Used as initializer. Called in <see cref="BotManager.Setup"/>.
        /// </summary>
        void Init(BotManager botManager);

        void BotManagerDisposed();
    }

    public delegate void UpdateReceivedEvent(object sender, UpdateReceivedEventArgs args);
}
