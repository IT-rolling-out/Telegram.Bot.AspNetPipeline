using System;
using System.Threading;
using System.Threading.Tasks;

namespace IRO.Tests.Telegram
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Task t;
            var tokenSource = new CancellationTokenSource();
      
            CancellationToken ct = tokenSource.Token;

        }
    }
}
