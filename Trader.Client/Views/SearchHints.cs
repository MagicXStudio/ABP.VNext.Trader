using System;
using System.Collections.ObjectModel;
using ReactiveUI;
using Trader.Domain.Infrastucture;
using Trader.Domain.Services;

namespace Trader.Client.Views
{
    public class SearchHints : ReactiveObject, IDisposable
    {
        private readonly ReadOnlyObservableCollection<string> _hints;
        private readonly IDisposable _cleanUp;
        private string _searchText;

        public SearchHints(IFileService tradeService, ISchedulerProvider schedulerProvider)
        {

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
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        public ReadOnlyObservableCollection<string> Hints => _hints;

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}