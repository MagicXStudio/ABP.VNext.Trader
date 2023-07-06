using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Trader.Domain.Model;

namespace Trader.Domain.Services
{
    public class MarketDataService : BaseService, IMarketDataService
    {
        private readonly Dictionary<string, IObservable<MarketData>> _prices = new Dictionary<string, IObservable<MarketData>>();

        public MarketDataService()
        {
            foreach (var item in TradeGenerator.CurrencyPairs)
            {
                _prices[item.Code] = GenerateStream(item).Replay(1).RefCount();
            }
        }

        private IObservable<MarketData> GenerateStream(CurrencyPair currencyPair)
        {
            IObservable<MarketData> items = Observable.Create<MarketData>(observer =>
            {
                MarketData initial = new MarketData(currencyPair.Code, 12, 12);
                return Observable.FromAsync(() => Task.FromResult(Directory.GetFiles(currencyPair.Code)))
                .Select(files => files.ToList())
                     .Subscribe(files =>
                     {
                         observer.OnNext(initial);
                     });
            });
            return items;
        }
        public IObservable<MarketData> Watch(string dir)
        {
            if (dir == null) throw new ArgumentNullException(nameof(dir));
            return (IObservable<MarketData>)_prices[dir];
        }
    }
}