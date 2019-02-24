using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IRO.Common.Abstractions;

namespace IRO.Samples.TelegramBotWithAsp
{
    /// <summary>
    /// Http client with simple queue of requests.
    /// It's not best solution, but useful for simple bots.
    /// </summary>
    public class QueuedHttpClient : HttpClient, IInformativeDisposable
    {
        readonly TimeSpan _timeout;

        readonly ConcurrentQueue<Action> _queue = new ConcurrentQueue<Action>();

        DateTime _lastDequeueTime = DateTime.MinValue;

        public bool IsDisposed { get; private set; }

        public QueuedHttpClient(TimeSpan timeout)
        {
            _timeout = timeout;

            var thread = new Thread(async () =>
            {
                while (!IsDisposed)
                {
                    try
                    {
                        await Task.Delay(_timeout);
                        DequeueNextIfNeeded();
                    }
                    catch { }
                }
            });
            thread.Start();
        }

        public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            TaskCompletionSource<bool> taskCompletionSource = null;
            taskCompletionSource = new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously
            );
            Action act = () => { taskCompletionSource?.TrySetResult(true); };
            _queue.Enqueue(act);
            cancellationToken.Register(() => { taskCompletionSource?.TrySetCanceled(); });
            await taskCompletionSource.Task;
            taskCompletionSource = null;
            return await base.SendAsync(request, cancellationToken);

        }

        void DequeueNextIfNeeded()
        {
            if (DateTime.Now - _lastDequeueTime > _timeout)
            {
                if (_queue.TryDequeue(out var act))
                {
                    act();
                    _lastDequeueTime = DateTime.Now;
                }
            }
        }

        public new void Dispose()
        {
            IsDisposed = true;
        }
    }
}
