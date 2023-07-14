using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Trader.Domain.Model;

namespace Trader.Domain.Services
{
    public interface IFileService
    {
        ObservableCollection<FileDetail> All { get; }
        IObservable<IEnumerable<FileDetail>> Live { get; }

        IObservable<IEnumerable<FileDetail>> Watch(string dir);
    }
}