using System;
using System.IO;

namespace Trader.Domain.Model
{
    public class DriveDetail
    {
        public DriveDetail(string drive)
        {
            DriveInfo = new DriveInfo(drive);
        }

        public DriveInfo DriveInfo { get; }

        public string Code => DriveInfo.Name;
        public long InitialPrice => DriveInfo.AvailableFreeSpace;
        public long DecimalPlaces => DriveInfo.TotalSize;
        public string TickFrequency => DriveInfo.DriveFormat;
        public decimal PipSize { get; }
        public long DefaultSpread => DriveInfo.TotalFreeSpace;

        #region Equality

        protected bool Equals(DriveDetail other)
        {
            return string.Equals(Code, other.Code);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DriveDetail)obj);
        }

        public override int GetHashCode()
        {
            return (Code != null ? Code.GetHashCode() : 0);
        }

        #endregion

        public override string ToString()
        {
            Span<Coords<int>> coordinates = stackalloc[]
            {
                new Coords<int> { X = 1, Y = 3 },
                new Coords<int> { X = 2, Y = 6 },
                new Coords<int> { X = 4, Y = 9 }
            };
            //堆栈中分配的内存块不受垃圾回收的影响，也不必通过 fixed 语句固定
            Span<int> numbers = stackalloc[] { 1, 2, 3, 4, 5, 6 };
            return $"Code: {Code}, DecimalPlaces: {DecimalPlaces}";
        }
    }
}