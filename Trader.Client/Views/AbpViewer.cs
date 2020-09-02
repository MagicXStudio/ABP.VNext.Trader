using ReactiveUI;
using System;
using System.Threading;
using System.Threading.Tasks;
using Trader.Client.Infrastucture;
using Trader.Domain.Algorithms;

namespace Trader.Client.Views
{
    /// <summary>
    ///多线程代码中的11个常见的问题 http://www.uml.org.cn/c%2B%2B/200902034.asp
    /// </summary>
    public class AbpViewer : ReactiveObject
    {
        public CountdownEvent Countdown { get; } = new CountdownEvent(100);

        public AbpViewer()
        {
            Parallel.For(1, 100, x =>
            {
                Task.Delay(1000);
                Counter = x;
            });
            DecrementAsync = async (x) =>
            {
                long result = await x;
                Counter += result;
            };
        }
        long counter;
        public long Counter
        {
            get => counter;
            set => this.RaiseAndSetIfChanged(ref counter, value);
        }

        public Command HelloCommand => new Command(() =>
                                                 {
                                                     Thread task = new Thread(() =>
                                                     {
                                                         Task.Delay(1000);
                                                         Counter = Interlocked.Decrement(ref counter);
                                                     });
                                                     task.Name = "T-Countdown";
                                                     task.Start();
                                                 });

        private Action<Task<long>> DecrementAsync { get; set; }

        public Command VolatileCommand => new Command(() =>
        {
            Counter--;
            DecrementAsync.Invoke(Task.FromResult(Counter));
        });
        public Command ExchangeCommand => new Command(async () =>
        {
            Interlocked.MemoryBarrier();
            Counter = await Fibonacci.FibAsync(26);
            Counter = Interlocked.Exchange(ref counter, Environment.TickCount);
        });
    }
}
