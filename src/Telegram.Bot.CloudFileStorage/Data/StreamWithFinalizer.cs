using System;
using System.IO;

namespace Telegram.Bot.CloudFileStorage.Data
{
    public class StreamWithFinalizer : IDisposable
    {
        public Stream Stream { get; }

        readonly Action _finalizer;

        public StreamWithFinalizer(Stream stream, Action finalizer)
        {
            Stream = stream;
            _finalizer = finalizer;
        }

        public void Dispose()
        {
            Stream?.Dispose();
            _finalizer?.Invoke();
        }
    }
}
