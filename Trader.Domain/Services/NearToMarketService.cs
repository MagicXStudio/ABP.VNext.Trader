using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using TradeExample.Annotations;
using Trader.Domain.Infrastucture;
using Trader.Domain.Model;

namespace Trader.Domain.Services
{
    public class NearToMarketService : BaseService, INearToMarketService
    {
        private readonly IFileService _tradeService;
        private readonly ILogger _logger;

        public NearToMarketService([NotNull] IFileService tradeService, ILogger logger)
        {
            _tradeService = tradeService ?? throw new ArgumentNullException(nameof(tradeService));
            _logger = logger;
        }

        public IObservable<IEnumerable<FileDetail>> Query(Func<decimal> percentFromMarket)
        {
            if (percentFromMarket == null) throw new ArgumentNullException(nameof(percentFromMarket));

            return Observable.Create<IEnumerable<FileDetail>>
                (observer =>
                 {
                     var locker = new object();

                     bool Predicate(FileDetail t) => Math.Abs(t.PercentFromMarket) <= percentFromMarket();

                     //re-evaluate filter periodically
                     var reevaluator = Observable.Interval(TimeSpan.FromMilliseconds(250))
                         .Synchronize(locker)
                         .Select(_ => (Func<FileDetail, bool>)Predicate)
                         .StartWith((Func<FileDetail, bool>)Predicate); ;

                     //filter on live trades matching % specified
                     return _tradeService.Live
                         .Synchronize(locker)
                         .Do(_ => { }, ex => _logger.Error(ex, ex.Message))
                         .SubscribeSafe(observer);
                 });
        }
    }
}