using System;
using System.Collections.Generic;
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
    public class FileService : BaseService, IFileService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ISchedulerProvider _schedulerProvider;

        private readonly IDisposable _cleanup;

        public FileService(ILogger logger, ISchedulerProvider schedulerProvider)
        {
            _logger = logger;
            _schedulerProvider = schedulerProvider;

            //emulate a trade service which asynchronously 
            IConnectableObservable<IChangeSet<FileDetail, long>> FileDetails = EnumerateFiles().Publish();

            //call AsObservableCache() so the cache can be directly exposed
            All = FileDetails.AsObservableCache();

            //create a derived cache  
            Live = FileDetails.Filter(trade => trade.Status == TradeStatus.Live).AsObservableCache();

            //log changes
            IDisposable loggerWriter = LogChanges();

            _cleanup = new CompositeDisposable(All, FileDetails.Connect(), loggerWriter);
        }

        private IObservable<IChangeSet<FileDetail, long>> EnumerateFiles()
        {
            //construct an cache datasource specifying that the primary key is Trade.Id
            return ObservableChangeSet.Create<FileDetail, long>(cache =>
            {
                Random random = new Random();

                //initally load some trades 

                TimeSpan RandomInterval() => TimeSpan.FromMilliseconds(random.Next(2500, 5000));
                // create a random number of trades at a random interval
                IDisposable tradeGenerator = _schedulerProvider.Background
                    .ScheduleRecurringAction(RandomInterval, () =>
                    {

                    });

                // close a random number of trades at a random interval
                IDisposable tradeCloser = _schedulerProvider.Background
                    .ScheduleRecurringAction(RandomInterval, () =>
                    {
                        int number = random.Next(1, 2);
                        cache.Edit(innerCache =>
                        {
                            FileDetail[] trades = innerCache.Items
                                .Where(trade => trade.Status == TradeStatus.Live)
                                .OrderBy(t => Guid.NewGuid()).Take(number).ToArray();
                            IEnumerable<FileDetail> toClose = trades.Select(trade => new FileDetail(trade, TradeStatus.Closed));
                            cache.AddOrUpdate(toClose);
                        });
                    });
                IDisposable expirer = cache
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
                                                    trade.DirectoryInfo))
                            .ForEachChange(change => _logger.Info(change.Current))
                            .Subscribe();

        }

        public IObservableCache<FileDetail, long> All { get; }

        public IObservableCache<FileDetail, long> Live { get; }

        public void Dispose()
        {
            _cleanup.Dispose();
        }

        public IObservable<FileDetail> Watch(string dir)
        {
            throw new NotImplementedException();
        }
    }
}