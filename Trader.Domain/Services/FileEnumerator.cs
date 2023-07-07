using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using Trader.Domain.Model;

namespace Trader.Domain.Services
{
    public class FileEnumerator : IDisposable
    {
        private readonly Random _random = new Random();
        private readonly IDisposable _cleanUp;
        private readonly IDictionary<string, FileDetail> Files = new Dictionary<string, FileDetail>();
        private readonly object _locker = new object();
        private int _counter = 0;

        private static string[] Directories => Directory.GetDirectories("C:\\");

        public FileEnumerator()
        {

        }

        public IEnumerable<FileDetail> EnumerateFiles(string dir)
        {
            FileDetail NewTrade(string file)
            {
                return new FileDetail(file, dir, TradeStatus.Live, BuyOrSell.Buy, 0.12m, Thread.CurrentThread.ManagedThreadId);
            }
            IEnumerable<FileDetail> result;
            lock (_locker)
            {
                result = Directory.EnumerateFiles(dir).Select((file) => NewTrade(file)).ToArray();
            }
            return result;
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}