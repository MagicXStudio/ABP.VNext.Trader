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

        private static string[] Drives => Directory.GetLogicalDrives();

        public static IEnumerable<DriveDetail> DriveDetails => Drives.Select((drive) => new DriveDetail(drive));

        public DirectorEnumerator(IDirectoryService marketDataService)
        {
            _cleanUp = DriveDetails
                                    .Select(drive => marketDataService.Watch(drive.Code)).Merge()
                                    .Synchronize(_locker)
                                    .Subscribe(dir =>
                                    {
                                        Directories[dir.Instrument] = dir;
                                    });
        }

        public IEnumerable<DirectoryDetail> EnumerateFiles(string drive)
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