using System;
using Trader.Domain.Model;

namespace Trader.Domain.Services
{
    public interface IDirectoryService
    {
        IObservable<DirectoryDetail> Watch(string drive);
    }
}