using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using TradeExample.Annotations;
using Trader.Domain.Infrastucture;

namespace Trader.Domain.Model
{
    public class TradesByTime : IDisposable, IEquatable<TradesByTime>
    {
        private readonly IDisposable _cleanUp;
        private readonly ReadOnlyObservableCollection<FileProxy> _data;

        public TradesByTime([NotNull] IObservable<IEnumerable<FileDetail>> group,
            ISchedulerProvider schedulerProvider)
        {
            _cleanUp = group
                .ObserveOn(schedulerProvider.MainThread)
                .Subscribe();
        }


        public TimePeriod Period { get; }

        public string Description
        {
            get
            {
                switch (Period)
                {
                    case TimePeriod.LastMinute:
                        return "Last Minute";
                    case TimePeriod.LastHour:
                        return "Last Hour";
                        ;
                    case TimePeriod.Older:
                        return "Old";
                    default:
                        return "Unknown";
                }
            }
        }

        public ReadOnlyObservableCollection<FileProxy> Data => _data;

        public void Dispose()
        {
            _cleanUp.Dispose();
        }

        #region Equality

        public bool Equals(TradesByTime other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Period == other.Period;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TradesByTime) obj);
        }

        public override int GetHashCode()
        {
            return (int) Period;
        }

        public static bool operator ==(TradesByTime left, TradesByTime right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(TradesByTime left, TradesByTime right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}