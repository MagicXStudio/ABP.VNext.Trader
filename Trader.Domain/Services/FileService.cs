using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Trader.Domain.Infrastucture;
using Trader.Domain.Model;

namespace Trader.Domain.Services
{
    public class FileService : BaseService, IFileService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ISchedulerProvider _schedulerProvider;
        private FileEnumerator FileEnumerator { get; }
        private readonly IDisposable _cleanup;

        public FileService(ILogger logger, ISchedulerProvider schedulerProvider)
        {
            _logger = logger;
            _schedulerProvider = schedulerProvider;
            FileEnumerator = new FileEnumerator();
            EnumerateFiles("C:\\").Subscribe((items) =>
            {
                All = new ObservableCollection<FileDetail>(items);
                Live = Observable.Return<IEnumerable<FileDetail>>(items);
            });

        }

        private IObservable<IEnumerable<FileDetail>> EnumerateFiles(string dir)
        {
            return Observable.FromAsync(() => Task.FromResult(FileEnumerator.EnumerateFiles(dir)));
        }

        public ObservableCollection<FileDetail> All { get; private set; }

        public IObservable<IEnumerable<FileDetail>> Live { get; private set; }

        public void Dispose()
        {
            _cleanup.Dispose();
        }

        public IObservable<IEnumerable<FileDetail>> Watch(string dir)
        {
            return Live;
        }
    }
}