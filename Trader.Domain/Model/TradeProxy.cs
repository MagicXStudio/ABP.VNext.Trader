using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData.Binding;

namespace Trader.Domain.Model
{
    public class FileProxy : AbstractNotifyPropertyChanged, IDisposable, IEquatable<FileProxy>
    {
        private readonly IDisposable _cleanUp;
        private readonly long _id;
        private readonly FileDetail _trade;
        private decimal _marketPrice;
        private decimal _pcFromMarketPrice;
        private bool _recent;

        public FileProxy(FileDetail trade)
        {
            _id = trade.Id;
            _trade = trade;

            var isRecent = DateTime.Now.Subtract(trade.Timestamp).TotalSeconds < 2;
            var recentIndicator = Disposable.Empty;

            if (isRecent)
            {
                Recent = true;
                recentIndicator = Observable.Timer(TimeSpan.FromSeconds(2))
                    .Subscribe(_ => Recent = false);
            }

            //market price changed is an observable on the trade object
            IDisposable priceRefresher = trade.MarketPriceChanged
                .Subscribe(_ =>
                {
                    MarketPrice = trade.MarketPrice;
                    PercentFromMarket = trade.PercentFromMarket;
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
            set => SetAndRaise(ref _recent, value);
        }

        public decimal MarketPrice
        {
            get => _marketPrice;
            set => SetAndRaise(ref _marketPrice, value);
        }

        public decimal PercentFromMarket
        {
            get => _pcFromMarketPrice;
            set => SetAndRaise(ref _pcFromMarketPrice, value);
        }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }


        #region Delegating Members

        public long Id => _trade.Id;

        public string CurrencyPair => _trade.CurrencyPair;

        public string Customer => _trade.DirectoryInfo.Name;

        public decimal Amount => _trade.Amount;

        public TradeStatus Status => _trade.Status;

        public DateTime Timestamp => _trade.Timestamp;

        public decimal TradePrice => _trade.TradePrice;

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
            return Equals((FileProxy) obj);
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