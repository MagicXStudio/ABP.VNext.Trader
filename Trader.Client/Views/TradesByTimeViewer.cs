using System;
using System.Collections.ObjectModel;
using ReactiveUI;
using Trader.Domain.Infrastucture;
using Trader.Domain.Model;
using Trader.Domain.Services;

namespace Trader.Client.Views
{
    public class TradesByTimeViewer : ReactiveObject, IDisposable
    {
        private readonly IDisposable _cleanUp;
        private readonly ReadOnlyObservableCollection<TradesByTime> _data;

        public TradesByTimeViewer(IFileService tradeService, ISchedulerProvider schedulerProvider)
        {

        }

        public ReadOnlyObservableCollection<TradesByTime> Data => _data;

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}