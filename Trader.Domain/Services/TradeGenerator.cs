using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.ExceptionServices;
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

        public static string[] Drives => Directory.GetLogicalDrives();

        public static IEnumerable<CurrencyPair> CurrencyPairs => Drives.Select((drive) => new CurrencyPair(drive, 1.6M, 4, 5M));

        public TradeGenerator(IMarketDataService marketDataService)
        {
            _cleanUp = CurrencyPairs
                                    .Select(currencypair => marketDataService.Watch(currencypair.Code)).Merge()
                                    .Synchronize(_locker)
                                    .Subscribe(md =>
                                    {
                                        _latestPrices[md.Instrument] = md;
                                    });
        }

        public IEnumerable<Trade> EnumerateFiles(string dir)
        {
            Trade NewTrade(string file)
            {
                var id = _counter++;
                var amount = (_random.Next(1, 2000) / 2) * (10 ^ _random.Next(1, 5));
                var buySell = _random.NextBoolean() ? BuyOrSell.Buy : BuyOrSell.Sell;
                return new Trade(id, dir, file, TradeStatus.Live, buySell, 0.12m, amount);
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