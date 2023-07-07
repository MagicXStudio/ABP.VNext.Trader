using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using DynamicData.PLinq;
using Trader.Client.Infrastucture;
using Trader.Domain.Infrastucture;
using Trader.Domain.Model;
using Trader.Domain.Services;

namespace Trader.Client.Views
{
    public class VisibleRowsViewer : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IDisposable _cleanUp;

        private readonly IVisibleRowsAccessor<FileProxy> _visibleRowsAccessor = new VisibleRowsAccessor<FileProxy>();
        private readonly ReadOnlyObservableCollection<FileProxy> _data;

        public VisibleRowsViewer(IFileService tradeService, ILogger logger, ISchedulerProvider schedulerProvider)
        {
            var loader = tradeService.All.Connect()
                .Transform(trade => new FileProxy(trade), new ParallelisationOptions(ParallelType.Ordered, 5))
                .Sort(SortExpressionComparer<FileProxy>.Descending(t => t.Timestamp), SortOptimisations.ComparesImmutableValuesOnly)
                .ObserveOn(schedulerProvider.MainThread)
                .Bind(out _data)   // update observable collection bindings
                .DisposeMany() //since TradeProxy is disposable dispose when no longer required
                .Subscribe();


            //NEED TO DO SOMETHING FUNKY

            var visibilityController = _visibleRowsAccessor.VisibleRows.Connect()
                                                .SubscribeMany(proxy =>
                                                {
                                                    //
                                                    return proxy.WhenValueChanged(p => p.Amount).Subscribe();
                                                }).Subscribe();

            _cleanUp = new CompositeDisposable(loader, _visibleRowsAccessor, visibilityController);
        }

        public IVisibleRowsAccessor<FileProxy> VisibleRowsAccessor => _visibleRowsAccessor;

        public ReadOnlyObservableCollection<FileProxy> Data => _data;


        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}