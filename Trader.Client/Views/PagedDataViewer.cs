using System;
using System.Collections.ObjectModel;
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
    public class PagedDataViewer : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IDisposable _cleanUp;
        private readonly ReadOnlyObservableCollection<FileProxy> _data;
        private string _searchText;

        public PagedDataViewer(IFileService tradeService, ISchedulerProvider schedulerProvider)
        {
            //build observable predicate from search text
            var filter = this.WhenValueChanged(t => t.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Select(BuildFilter);

            //build observable sort comparer
            var sort = SortParameters.WhenValueChanged(t => t.SelectedItem)
                .Select(prop => prop.Comparer)
                .ObserveOn(schedulerProvider.Background);

            var pager = PageParameters.WhenChanged(vm=>vm.PageSize,vm=>vm.CurrentPage, (_,size, pge) => new PageRequest(pge, size))
                .StartWith(new PageRequest(1, 25))
                .DistinctUntilChanged()
                .Sample(TimeSpan.FromMilliseconds(100));
            
            // filter, sort, page and bind to observable collection
            _cleanUp = tradeService.All.Connect()
                .Filter(filter) // apply user filter
                .Transform(trade => new FileProxy(trade), new ParallelisationOptions(ParallelType.Ordered, 5))
                .Sort(sort, SortOptimisations.ComparesImmutableValuesOnly)
                .Page(pager)
                .ObserveOn(schedulerProvider.MainThread)
                .Do(changes => PageParameters.Update(changes.Response))
                .Bind(out _data)        // update observable collection bindings
                .DisposeMany()          // dispose when no longer required
                .Subscribe();
        }

        private static Func<FileDetail, bool> BuildFilter(string searchText)
        {
            if (string.IsNullOrEmpty(searchText)) return trade => true;

            return t => t.CurrencyPair.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                                            t.DirectoryInfo.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase);
        }

        public string SearchText
        {
            get => _searchText;
            set => SetAndRaise(ref _searchText, value);
        }

        public ReadOnlyObservableCollection<FileProxy> Data => _data;

        public PageParameterData PageParameters { get;} = new PageParameterData(1,25);

        public SortParameterData SortParameters { get; } = new SortParameterData();

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}