using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Trader.Domain.Model;

namespace Trader.Domain.Services
{
    public class FileSearchJob : BaseService, IDisposable
    {
        private readonly IDisposable _job;

        public FileSearchJob(IFileService fileService, IDirectoryService directoryService)
        {
            _job = fileService.All
                .Subscribe(items =>
                               {
                                   object locker = new object();

                               });
        }

        private void Update(IEnumerable<FileDetail> items, decimal price)
        {
            items.ForEach(t => t.SetMarketPrice(price));
        }

       
    }
}
