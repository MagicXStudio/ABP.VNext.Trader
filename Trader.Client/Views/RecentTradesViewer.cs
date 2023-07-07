using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Trader.Domain.Infrastucture;
using Trader.Domain.Model;
using Trader.Domain.Services;

namespace Trader.Client.Views
{
    public class RecentTradesViewer : ReactiveObject, IDisposable
    {
        private readonly IDisposable _cleanUp;
        private readonly ReadOnlyObservableCollection<FileProxy> _data;
        private readonly ILogger _logger;

        public RecentTradesViewer(ILogger logger, IFileService tradeService, ISchedulerProvider schedulerProvider)
        {
            _logger = logger;

            _cleanUp = tradeService.All
                .Skip(0)
                .ObserveOn(schedulerProvider.MainThread)
                .Subscribe((items) =>
                {


                });
        }

        public ReadOnlyObservableCollection<FileProxy> Data => _data;

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}