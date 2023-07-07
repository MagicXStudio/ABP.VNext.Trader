using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Trader.Domain.Infrastucture;
using Trader.Domain.Services;

namespace Trader.Client.Views
{
    public class TradesByPercentViewer : ReactiveObject, IDisposable
    {
        private readonly IDisposable _cleanUp;
        private readonly ReadOnlyObservableCollection<Domain.Model.TradesByPercentDiff> _data;

        public TradesByPercentViewer(INearToMarketService nearToMarketService, ISchedulerProvider schedulerProvider, ILogger logger)
        {
            var locker = new object();
            var grouperRefresher = Observable.Interval(TimeSpan.FromSeconds(1))
                .Synchronize(locker)
                .Select(_ => Unit.Default);

            _cleanUp = nearToMarketService.Query(() => 4)
                .Synchronize(locker)
                .GroupBy(trade => (int)Math.Truncate(25.2))
                .ObserveOn(schedulerProvider.MainThread)
                .Subscribe(items => {; }, ex => logger.Error(ex, ex.Message));
        }

        public ReadOnlyObservableCollection<Domain.Model.TradesByPercentDiff> Data => _data;

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}