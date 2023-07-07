using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using ReactiveUI;
using Trader.Domain.Infrastucture;
using Trader.Domain.Model;
using Trader.Domain.Services;

namespace Trader.Client.Views
{
    public class PositionsViewer : ReactiveObject, IDisposable
    {
        private readonly ReadOnlyObservableCollection<CurrencyPairPosition> _data;
        private readonly IDisposable _cleanUp;

        public PositionsViewer(IFileService tradeService, ISchedulerProvider schedulerProvider)
        {
            _cleanUp = tradeService.Live
                .GroupBy(trade => trade.First())
                .ObserveOn(schedulerProvider.MainThread)
                .Subscribe((items) =>
                {


                });
        }

        public ReadOnlyObservableCollection<CurrencyPairPosition> Data => _data;

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}
