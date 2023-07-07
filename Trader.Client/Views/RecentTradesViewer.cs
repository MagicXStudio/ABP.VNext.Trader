using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using Trader.Domain.Infrastucture;
using Trader.Domain.Model;
using Trader.Domain.Services;

namespace Trader.Client.Views
{
    public class RecentTradesViewer : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IDisposable _cleanUp;
        private readonly ReadOnlyObservableCollection<FileProxy> _data;
        private readonly ILogger _logger;

        public RecentTradesViewer(ILogger logger, IFileService tradeService, ISchedulerProvider schedulerProvider)
        {
            _logger = logger;

            _cleanUp = tradeService.All.Connect()
                .SkipInitial()
                .ExpireAfter(trade => TimeSpan.FromSeconds(30))
                .Transform(trade => new FileProxy(trade))
                .Sort(SortExpressionComparer<FileProxy>.Descending(t => t.Timestamp), SortOptimisations.ComparesImmutableValuesOnly)
                .ObserveOn(schedulerProvider.MainThread)
                .Bind(out _data)
                .DisposeMany()
                .Subscribe();
        }

        public ReadOnlyObservableCollection<FileProxy> Data => _data;

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}