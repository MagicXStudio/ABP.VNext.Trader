using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Trader.Domain.Infrastucture;
using Trader.Domain.Model;
using Trader.Domain.Services;

namespace Trader.Client.Views
{
    public class LiveTradesViewer : ReactiveObject, IDisposable
    {
        private readonly IDisposable _cleanUp;
        private ReadOnlyObservableCollection<FileDetail> _data;
        private bool _paused;

        public LiveTradesViewer(IFileService fileService, SearchHints searchHints, ISchedulerProvider schedulerProvider)
        {
            SearchHints = searchHints;
            var loader = fileService.Live
                .ObserveOn(schedulerProvider.MainThread)
                .Subscribe((items) =>
                {
                    _data = new ReadOnlyObservableCollection<FileDetail>(new ObservableCollection<FileDetail>(items));
                });
            _cleanUp = new CompositeDisposable(loader, searchHints);
        }

        private Func<FileDetail, bool> BuildFilter(string searchText)
        {
            if (string.IsNullOrEmpty(searchText)) return trade => true;

            return t => t.CurrencyPair.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                            || t.DirectoryInfo.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase);
        }

        public ReadOnlyObservableCollection<FileDetail> Data => _data;

        public SearchHints SearchHints { get; }


        public bool Paused
        {
            get => _paused;
            set => this.RaiseAndSetIfChanged(ref _paused, value);
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}