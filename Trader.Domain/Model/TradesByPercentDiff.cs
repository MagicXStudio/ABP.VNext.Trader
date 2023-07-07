using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using TradeExample.Annotations;
using Trader.Domain.Infrastucture;

namespace Trader.Domain.Model
{
    public class TradesByPercentDiff : IDisposable, IEquatable<TradesByPercentDiff>
    {
        private readonly IDisposable _cleanUp;
        private readonly IEnumerable<FileDetail> _group;

        public TradesByPercentDiff([NotNull] IObservable<IEnumerable<FileDetail>> items, [NotNull] ISchedulerProvider schedulerProvider, ILogger logger)
        {
            if (schedulerProvider == null) throw new ArgumentNullException(nameof(schedulerProvider));

            _cleanUp = items
                .ObserveOn(schedulerProvider.MainThread)
                .Subscribe(_ => { }, ex => logger.Error(ex, "Error in TradesByPercentDiff"));
            Data = items;
        }

        public int PercentBand { get; }



        public IObservable<IEnumerable<FileDetail>> Data { get; }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }

        #region Equality

        public bool Equals(TradesByPercentDiff other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return PercentBand == other.PercentBand;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TradesByPercentDiff)obj);
        }

        public override int GetHashCode()
        {
            return PercentBand;
        }

        public static bool operator ==(TradesByPercentDiff left, TradesByPercentDiff right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TradesByPercentDiff left, TradesByPercentDiff right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}