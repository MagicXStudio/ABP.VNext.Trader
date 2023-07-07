using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Trader.Domain.Model;

namespace Trader.Domain.Services
{
    public class DirectoryService : BaseService, IDirectoryService
    {
        private readonly Dictionary<string, IObservable<DirectoryDetail>> Directories = new Dictionary<string, IObservable<DirectoryDetail>>();

        public DirectoryService()
        {
            foreach (DriveDetail item in DirectorEnumerator.DriveDetails)
            {
                Directories[item.Code] = GenerateStream(item).Replay(1).RefCount();
            }
        }

        private IObservable<DirectoryDetail> GenerateStream(DriveDetail drive)
        {
            IObservable<DirectoryDetail> items = Observable.Create<DirectoryDetail>(observer =>
            {
                DirectoryDetail initial = new DirectoryDetail("",drive.Code, 12, 12);
                return Observable.FromAsync(() => Task.FromResult(Directory.GetFiles(drive.Code)))
                .Select(files => files.ToList())
                     .Subscribe(files =>
                     {
                         observer.OnNext(initial);
                     });
            });
            return items;
        }
        public IObservable<DirectoryDetail> Watch(string drive)
        {
            if (drive == null) throw new ArgumentNullException(nameof(drive));
            return Directories[drive];
        }
    }
}