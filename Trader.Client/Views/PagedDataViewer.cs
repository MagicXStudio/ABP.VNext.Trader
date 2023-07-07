using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Trader.Client.Infrastucture;
using Trader.Domain.Infrastucture;
using Trader.Domain.Model;
using Trader.Domain.Services;

namespace Trader.Client.Views
{
    public class PagedDataViewer : ReactiveObject, IDisposable
    {
        private readonly IDisposable _cleanUp;
        private readonly ReadOnlyObservableCollection<FileProxy> _data;
        private string _searchText;

        public PagedDataViewer(IFileService tradeService, ISchedulerProvider schedulerProvider)
        {
            //build observable predicate from search text
            var filter = tradeService.Live
                .Throttle(TimeSpan.FromMilliseconds(250))
                .Select((items) => { return items; });

            //build observable sort comparer
            var sort = tradeService.Live
                .Select((items) => { return items; })
                .ObserveOn(schedulerProvider.Background);

            var pager = tradeService.Live.Skip(10)
                .DistinctUntilChanged()
                .Sample(TimeSpan.FromMilliseconds(100));
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
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        public ReadOnlyObservableCollection<FileProxy> Data => _data;

        public PageParameterData PageParameters { get; } = new PageParameterData(1, 25);


        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}