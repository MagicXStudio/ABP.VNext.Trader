using ReactiveUI;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Trader.Domain.Model
{
    public class FileProxy : ReactiveObject, IDisposable, IEquatable<FileProxy>
    {
        private readonly IDisposable _cleanUp;
        private readonly long _id;
        private readonly FileDetail _item;
        private decimal _marketPrice;
        private decimal _pcFromMarketPrice;
        private bool _recent;

        public FileProxy(FileDetail item)
        {
            _id = item.Id;
            _item = item;
            var isRecent = DateTime.Now.Subtract(item.Timestamp).TotalSeconds < 2;
            var recentIndicator = Disposable.Empty;

            if (isRecent)
            {
                Recent = true;
                recentIndicator = Observable.Timer(TimeSpan.FromSeconds(2))
                    .Subscribe(_ => Recent = false);
            }

            //market price changed is an observable on the trade object
            IDisposable priceRefresher = item.MarketPriceChanged
                .Subscribe(_ =>
                {
                    MarketPrice = item.MarketPrice;
                    PercentFromMarket = item.PercentFromMarket;
                });

            _cleanUp = Disposable.Create(() =>
            {
                recentIndicator.Dispose();
                priceRefresher.Dispose();
            });
        }

        public bool Recent
        {
            get => _recent;
            set => this.RaiseAndSetIfChanged(ref _recent, value);
        }

        public decimal MarketPrice
        {
            get => _marketPrice;
            set => this.RaiseAndSetIfChanged(ref _marketPrice, value);
        }

        public decimal PercentFromMarket
        {
            get => _pcFromMarketPrice;
            set => this.RaiseAndSetIfChanged(ref _pcFromMarketPrice, value);
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
        #region Delegating Members

        public long Id => _item.Id;

        public string DirectoryName => _item.DirectoryInfo.Name;

        public string Name => _item.Name;

        public bool Exists => _item.FileInfo.Exists;

        public DateTime Timestamp => _item.Timestamp;

        public decimal TradePrice => _item.TradePrice;

        #endregion

        #region Equaility Members

        public bool Equals(FileProxy other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _id == other._id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((FileProxy)obj);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public static bool operator ==(FileProxy left, FileProxy right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(FileProxy left, FileProxy right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}