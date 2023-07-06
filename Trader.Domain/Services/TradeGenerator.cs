using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using DynamicData.Kernel;
using Trader.Domain.Model;

namespace Trader.Domain.Services
{
    public class TradeGenerator : IDisposable
    {
        private readonly Random _random = new Random();
        private readonly IDisposable _cleanUp;
        private readonly IDictionary<string, MarketData> _latestPrices = new Dictionary<string, MarketData>();
        private readonly object _locker = new object();
        private int _counter = 0;

        private static string[] Drives => Directory.GetLogicalDrives();

        public static IEnumerable<CurrencyPair> CurrencyPairs => Drives.Select((drive) => new CurrencyPair(drive));

        public TradeGenerator(IMarketDataService marketDataService)
        {
            _cleanUp = CurrencyPairs
                                    .Select(currencypair => marketDataService.Watch(currencypair.Code)).Merge()
                                    .Synchronize(_locker)
                                    .Subscribe(data =>
                                    {
                                        _latestPrices[data.Instrument] = data;
                                    });
        }

        public IEnumerable<Trade> EnumerateFiles(string dir)
        {
            Trade NewTrade(string file)
            {
                return new Trade(_counter++, dir, file, TradeStatus.Live, BuyOrSell.Buy, 0.12m, Thread.CurrentThread.ManagedThreadId);
            }
            IEnumerable<Trade> result;
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