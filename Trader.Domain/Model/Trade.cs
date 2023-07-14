using System;
using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Trader.Domain.Model
{
    public class FileDetail : IDisposable, IEquatable<FileDetail>
    {
        private readonly ISubject<decimal> _marketPriceChangedSubject = new ReplaySubject<decimal>(1);

        public long Id { get; }

        public string Name { get; set; }
        public FileInfo FileInfo { get; }
        public decimal TradePrice => FileInfo.Length;
        public decimal MarketPrice { get; private set; }
        public decimal PercentFromMarket { get; private set; }
        public decimal Amount { get; }
        public BuyOrSell BuyOrSell { get; }
        public TradeStatus Status { get; }
        public DateTime Timestamp => DirectoryInfo.CreationTime;

        public DirectoryInfo DirectoryInfo { get; }

        public FileDetail(string name, string dir, TradeStatus status, BuyOrSell buyOrSell, decimal tradePrice, decimal amount, decimal marketPrice = 0)
        {
            Name = name;
            DirectoryInfo = new DirectoryInfo(dir);
            FileInfo = new FileInfo(name);
            Status = status;
            MarketPrice = marketPrice;
            Amount = amount;
            BuyOrSell = buyOrSell;
        }

        public void SetMarketPrice(decimal marketPrice)
        {
            MarketPrice = marketPrice;
            PercentFromMarket = Math.Round(((TradePrice - MarketPrice) / MarketPrice) * 100, 4);
            ;
            _marketPriceChangedSubject.OnNext(marketPrice);
        }

        public IObservable<decimal> MarketPriceChanged => _marketPriceChangedSubject.AsObservable();

        #region Equality Members

        public bool Equals(FileDetail other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FileDetail)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(FileDetail left, FileDetail right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FileDetail left, FileDetail right)
        {
            return !Equals(left, right);
        }

        #endregion

        public void Dispose()
        {
            _marketPriceChangedSubject.OnCompleted();
        }
    }
}