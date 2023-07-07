using ReactiveUI;
using System;

namespace Trader.Domain.Model
{
    public class CurrencyPairPosition: ReactiveObject,  IDisposable, IEquatable<CurrencyPairPosition>
    {
        private readonly IDisposable _cleanUp;
        private TradesPosition _position;

        public CurrencyPairPosition(IObservable<FileDetail> tradesByCurrencyPair)
        {
        }

        public TradesPosition Position
        {
            get => _position;
            set => this.RaiseAndSetIfChanged(ref  _position,value);
        }

        public string CurrencyPair { get; }

        #region Equality Members

        public bool Equals(CurrencyPairPosition other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(CurrencyPair, other.CurrencyPair);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CurrencyPairPosition) obj);
        }

        public override int GetHashCode()
        {
            return (CurrencyPair != null ? CurrencyPair.GetHashCode() : 0);
        }

        public static bool operator ==(CurrencyPairPosition left, CurrencyPairPosition right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CurrencyPairPosition left, CurrencyPairPosition right)
        {
            return !Equals(left, right);
        }

        #endregion

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}