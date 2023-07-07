using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using Trader.Domain.Model;

namespace Trader.Domain.Services
{
    public class DirectorEnumerator : IDisposable
    {
        private readonly Random _random = new Random();
        private readonly IDisposable _cleanUp;
        private readonly IDictionary<string, DirectoryDetail> Directories = new Dictionary<string, DirectoryDetail>();
        private readonly object _locker = new object();
        private int _counter = 0;

        public DirectorEnumerator()
        {

        }

        public IEnumerable<DirectoryDetail> EnumerateDirectories(string drive)
        {
            DirectoryDetail NewTrade(string name)
            {
                return new DirectoryDetail(name, drive, 0, Thread.CurrentThread.ManagedThreadId);
            }
            IEnumerable<DirectoryDetail> result;
            lock (_locker)
            {
                result = Directory.EnumerateDirectories(drive).Select((dir) => NewTrade(dir));
            }
            return result;
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}