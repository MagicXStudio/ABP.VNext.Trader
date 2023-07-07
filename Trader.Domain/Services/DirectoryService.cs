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
        private readonly Dictionary<string, IObservable<IEnumerable<DirectoryDetail>>> Directories = new Dictionary<string, IObservable<IEnumerable<DirectoryDetail>>>();

        private static string[] Drives => Directory.GetLogicalDrives();

        public static IEnumerable<DriveDetail> DriveDetails => Drives.Select((drive) => new DriveDetail(drive));

        public DirectoryService()
        {
            foreach (DriveDetail item in DriveDetails)
            {
                Directories[item.Code] = GetDirectories(item);
            }
        }

        private IObservable<IEnumerable<DirectoryDetail>> GetDirectories(DriveDetail drive)
        {
            IEnumerable<DirectoryDetail> items = Directory.GetDirectories(drive.Code).Select((dir) => new DirectoryDetail(dir, drive.Code, 12, 12));
            return Observable.FromAsync(() => Task.FromResult(items)); ;
        }
        public IObservable<IEnumerable<DirectoryDetail>> Watch(string drive)
        {
            if (drive == null) throw new ArgumentNullException(nameof(drive));
            return Directories[drive];
        }
    }
}