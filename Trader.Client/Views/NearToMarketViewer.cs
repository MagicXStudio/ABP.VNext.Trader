using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using ReactiveUI;
using Trader.Domain.Infrastucture;
using Trader.Domain.Model;
using Trader.Domain.Services;

namespace Trader.Client.Views
{
    public class NearToMarketViewer : ReactiveObject, IDisposable
    {
        private readonly IDisposable _cleanUp;
        private readonly ReadOnlyObservableCollection<FileProxy> _data;
        private double _nearToMarketPercent = 0.05D;

        public NearToMarketViewer(INearToMarketService nearToMarketService, ISchedulerProvider schedulerProvider, ILogger logger)
        {
            _cleanUp = nearToMarketService.Query(() => (decimal)NearToMarketPercent)
                .ObserveOn(schedulerProvider.MainThread)
                .Subscribe((items) =>
                {

                }, ex => logger.Error(ex, "Error in near to market viewer"));
        }

        public double NearToMarketPercent
        {
            get => _nearToMarketPercent;
            set => this.RaiseAndSetIfChanged(ref _nearToMarketPercent, value);
        }

        public ReadOnlyObservableCollection<FileProxy> Data => _data;

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}