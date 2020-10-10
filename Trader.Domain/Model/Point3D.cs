using System;

namespace Trader.Domain.Model
{

    /// <summary>
    /// https://docs.microsoft.com/zh-cn/dotnet/csharp/write-safe-efficient-code
    /// </summary>
    public struct Point3D
    {
        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double X { readonly get; set; }
        public double Y { readonly get; set; }
        public double Z { readonly get; set; }

        public readonly double Distance => Math.Sqrt((X * X) + (Y * Y) + (Z * Z));

        public override readonly string ToString() => $"{X}, {Y}, {Z}";

        private static Point3D origin = new Point3D(0, 0, 0);
        private static Point3D max = new Point3D(double.MaxValue, double.MaxValue, double.MaxValue);
        private static Point3D min = new Point3D(double.MinValue, double.MinValue, double.MinValue);

        /// <summary>
        /// 不希望调用方修改原点，所以应该通过 ref readonly 
        /// </summary>
        public static ref readonly Point3D Origin => ref origin;
        public static ref readonly Point3D Max => ref max;
        public static ref readonly Point3D Min => ref min;

        
    }
}
