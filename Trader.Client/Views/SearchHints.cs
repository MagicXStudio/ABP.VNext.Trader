using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using DynamicData.Binding;
using Trader.Domain.Infrastucture;
using Trader.Domain.Model;
using Trader.Domain.Services;

namespace Trader.Client.Views
{
    public class SearchHints : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly ReadOnlyObservableCollection<string> _hints;
        private readonly IDisposable _cleanUp;
        private string _searchText;

        public SearchHints(IFileService tradeService, ISchedulerProvider schedulerProvider)
        {
            //build a predicate when SearchText changes
            IObservable<Func<string, bool>> filter = this.WhenValueChanged(t => t.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Select(BuildFilter);

            //share the connection
            IConnectableObservable<IChangeSet<FileDetail, long>> shared = tradeService.All.Connect().Publish();
            //distinct observable of customers
            IObservable<IDistinctChangeSet<string>> customers = shared.DistinctValues(trade => trade.DirectoryInfo.Name);
            //distinct observable of currency pairs
            IObservable<IDistinctChangeSet<string>> currencypairs = shared.DistinctValues(trade => trade.CurrencyPair);

            //observe customers and currency pairs using OR operator, and bind to the observable collection
            IDisposable loader = customers.Or(currencypairs)
                .Filter(filter)     //filter strings
                .Sort(SortExpressionComparer<string>.Ascending(str => str))
                .ObserveOn(schedulerProvider.MainThread)
                .Bind(out _hints)       //bind to hints list
                .Subscribe();
            _cleanUp = new CompositeDisposable(loader, shared.Connect());
        }

        private Func<string, bool> BuildFilter(string searchText)
        {
            if (string.IsNullOrEmpty(searchText)) return trade => true;

            return str => str.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                        str.Contains(searchText, StringComparison.OrdinalIgnoreCase);
        }

        public string SearchText
        {
            get => _searchText;
            set => SetAndRaise(ref _searchText, value);
        }

        public ReadOnlyObservableCollection<string> Hints => _hints;

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}