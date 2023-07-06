using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
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
            return Observable.Create<MarketData>(observer =>
            {
                int spread = currencyPair.DefaultSpread;
                decimal midRate = currencyPair.InitialPrice;
                decimal bid = midRate - (spread * currencyPair.PipSize);
                decimal offer = midRate + (spread * currencyPair.PipSize);
                MarketData initial = new MarketData(currencyPair.Code, bid, offer);

                var currentPrice = initial;
                observer.OnNext(initial);

                var random = new Random();
                //for a given period, move prices by up to 5 pips
                return Observable.Interval(TimeSpan.FromSeconds(1 / 30))
                    .Select(_ => random.Next(1, 5))
                    .Subscribe(pips =>
                    {
                        //move up or down between 1 and 5 pips
                        var adjustment = Math.Round(pips * currencyPair.PipSize, currencyPair.DecimalPlaces);
                        currentPrice = random.NextDouble() > 0.5
                                        ? currentPrice + adjustment
                                        : currentPrice - adjustment;
                        observer.OnNext(currentPrice);

                    });
            });
        }
        public IObservable<MarketData> Watch(string dir)
        {
            if (dir == null) throw new ArgumentNullException(nameof(dir));
            return (IObservable<MarketData>)_prices[dir];
        }
    }
}