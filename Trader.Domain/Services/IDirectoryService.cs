using System;
using System.Collections.Generic;
using Trader.Domain.Model;

namespace Trader.Domain.Services
{
    public interface IDirectoryService
    {
        IObservable<IEnumerable<DirectoryDetail>> Watch(string drive);
    }
}