using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using DynamicData.Kernel;
using Trader.Domain.Infrastucture;
using Trader.Domain.Model;

namespace Trader.Domain.Services
{
    public class TradeService : BaseService, ITradeService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly TradeGenerator _tradeGenerator;
        private readonly ISchedulerProvider _schedulerProvider;

        private readonly IDisposable _cleanup;

        public TradeService(ILogger logger, TradeGenerator tradeGenerator, ISchedulerProvider schedulerProvider)
        {
            _logger = logger;
            _tradeGenerator = tradeGenerator;
            _schedulerProvider = schedulerProvider;

            //emulate a trade service which asynchronously 
            IConnectableObservable<IChangeSet<Trade, long>> tradesData = GenerateTradesAndMaintainCache().Publish();

            //call AsObservableCache() so the cache can be directly exposed
            All = tradesData.AsObservableCache();

            //create a derived cache  
            Live = tradesData.Filter(trade => trade.Status == TradeStatus.Live).AsObservableCache();

            //log changes
            IDisposable loggerWriter = LogChanges();

            _cleanup = new CompositeDisposable(All, tradesData.Connect(), loggerWriter);
        }

        private IObservable<IChangeSet<Trade, long>> GenerateTradesAndMaintainCache()
        {
            //construct an cache datasource specifying that the primary key is Trade.Id
            return ObservableChangeSet.Create<Trade, long>(cache =>
            {
                Random random = new Random();
                
                //initally load some trades 
                cache.AddOrUpdate(_tradeGenerator.EnumerateFiles(Environment.CurrentDirectory));

                TimeSpan RandomInterval() => TimeSpan.FromMilliseconds(random.Next(2500, 5000));
                // create a random number of trades at a random interval
                IDisposable tradeGenerator = _schedulerProvider.Background
                    .ScheduleRecurringAction(RandomInterval, () =>
                    {
                        var trades = _tradeGenerator.EnumerateFiles(Environment.CurrentDirectory);
                        cache.AddOrUpdate(trades);
                    });

                // close a random number of trades at a random interval
                IDisposable tradeCloser = _schedulerProvider.Background
                    .ScheduleRecurringAction(RandomInterval, () =>
                    {
                        int number = random.Next(1, 2);
                        cache.Edit(innerCache =>
                        {
                            var trades = innerCache.Items
                                .Where(trade => trade.Status == TradeStatus.Live)
                                .OrderBy(t => Guid.NewGuid()).Take(number).ToArray();

                            var toClose = trades.Select(trade => new Trade(trade, TradeStatus.Closed));

                            cache.AddOrUpdate(toClose);
                        });
                    });

                //expire closed items from the cache to avoid unbounded data
                var expirer = cache
                    .ExpireAfter(t => t.Status == TradeStatus.Closed ? TimeSpan.FromMinutes(1) : (TimeSpan?)null, TimeSpan.FromMinutes(1), _schedulerProvider.Background)
                    .Subscribe(x => _logger.Info("{0} filled trades have been removed from memory", x.Count()));

                return new CompositeDisposable(tradeGenerator, tradeCloser, expirer);
            }, trade => trade.Id);
        }

        private IDisposable LogChanges()
        {
            const string messageTemplate = "{0} {1} {2} ({4}). Status = {3}";
            return All.Connect().Skip(1)
                            .WhereReasonsAre(ChangeReason.Add, ChangeReason.Update)
                            .Cast(trade => string.Format(messageTemplate,
                                                    trade.BuyOrSell,
                                                    trade.Amount,
                                                    trade.CurrencyPair,
                                                    trade.Status,
                                                    trade.Customer))
                            .ForEachChange(change => _logger.Info(change.Current))
                            .Subscribe();

        }

        public IObservableCache<Trade, long> All { get; }

        public IObservableCache<Trade, long> Live { get; }

        public void Dispose()
        {
            _cleanup.Dispose();
        }
    }
}