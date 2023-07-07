using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using Trader.Domain.Model;

namespace Trader.Domain.Services
{
    public class TradePriceUpdateJob : BaseService, IDisposable
    {
        private readonly IDisposable _job;

        public TradePriceUpdateJob(IFileService tradeService, IDirectoryService marketDataService)
        {
            _job = tradeService.All
                .Connect(trade => trade.Status == TradeStatus.Live)
                .Group(trade => trade.CurrencyPair)
                .SubscribeMany(groupedData =>
                               {
                                   object locker = new object();
                                   decimal latestPrice = 0;

                                   //subscribe to price and update trades with the latest price
                                   IDisposable priceHasChanged = marketDataService.Watch(groupedData.Key)
                                       .Synchronize(locker)
                                       .Subscribe(items =>
                                                  {
                                                      latestPrice = items.Count();
                                                      UpdateTradesWithPrice(groupedData.Cache.Items, latestPrice);
                                                  });

                                   //connect to data changes and update with the latest price
                                   IDisposable dataHasChanged = groupedData.Cache.Connect()
                                       .WhereReasonsAre(ChangeReason.Add, ChangeReason.Update)
                                       .Synchronize(locker)
                                       .Subscribe(changes => UpdateTradesWithPrice(changes.Select(change => change.Current), latestPrice));

                                   return new CompositeDisposable(priceHasChanged, dataHasChanged);

                               })
                .Subscribe();
        }

        private void UpdateTradesWithPrice(IEnumerable<FileDetail> items, decimal price)
        {
            items.ForEach(t=>t.SetMarketPrice(price));
        }

        public void Dispose()
        {
            _job.Dispose();
        }
    }
}
