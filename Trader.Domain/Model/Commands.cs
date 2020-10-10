using System;

namespace Trader.Domain.Model
{
    public static class Commands
    {
        static Commands()
        {

        }
        public static Tuple<int, int> Login { get => Tuple.Create(6, 3); }
    }
}
