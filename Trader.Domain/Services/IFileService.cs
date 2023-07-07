using DynamicData;
using System;
using Trader.Domain.Model;

namespace Trader.Domain.Services
{
    public interface IFileService
    {
        IObservableCache<FileDetail, long> All { get; }
        IObservableCache<FileDetail, long> Live { get; }

        IObservable<FileDetail> Watch(string dir);
    }
}