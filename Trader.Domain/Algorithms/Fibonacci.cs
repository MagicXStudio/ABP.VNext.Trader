using System;

namespace Trader.Domain.Algorithms
{
    /// <summary>
    /// 
    /// </summary>
    public static class Fibonacci
    {
        public static int Fib(int x)
        {
            if (x < 0)
                throw new ArgumentException("Less negativity please!", nameof(x));
            return Fib1(x).current;
        }

        /// <summary>
        ///C# 7 https://blogs.msdn.microsoft.com/dotnet/2016/08/24/whats-new-in-csharp-7-0/
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private static (int current, int previous) Fib1(int i)
        {
            if (i == 0) return (1, 0);
            (int p, int pp) = Fib1(i - 1);
            return (p + pp, p);
        }
    }
}
