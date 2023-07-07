using System;
using System.Collections.Generic;
using Trader.Domain.Model;

namespace Trader.Domain.Services
{
    public interface IFileService
    {
        IObservable<IEnumerable<FileDetail>> All { get; }
        IObservable<IEnumerable<FileDetail>> Live { get; }

        IObservable<IEnumerable<FileDetail>> Watch(string dir);
    }
}