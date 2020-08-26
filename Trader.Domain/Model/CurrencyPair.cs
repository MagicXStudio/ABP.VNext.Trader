using System;

namespace Trader.Domain.Model
{
    public class CurrencyPair
    {
        public CurrencyPair(string code, decimal startingPrice, int decimalPlaces, decimal tickFrequency, int defaultSpread = 8)
        {
            Code = code;
            InitialPrice = startingPrice;
            DecimalPlaces = decimalPlaces;
            TickFrequency = tickFrequency;
            DefaultSpread = defaultSpread;
            PipSize = (decimal)Math.Pow(10, -decimalPlaces);
        }

        public string Code { get; }
        public decimal InitialPrice { get; }
        public int DecimalPlaces { get; }
        public decimal TickFrequency { get; }
        public decimal PipSize { get; }
        public int DefaultSpread { get; }

        #region Equality

        protected bool Equals(CurrencyPair other)
        {
            return string.Equals(Code, other.Code);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CurrencyPair)obj);
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