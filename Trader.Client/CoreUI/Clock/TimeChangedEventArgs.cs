using System;
using System.Windows;

namespace Trader.Client.CoreUI.Clock
{
    public sealed class TimeChangedEventArgs : RoutedEventArgs
    {
        public DateTime OldTime { get; }
        public DateTime NewTime { get; }

        public TimeChangedEventArgs(RoutedEvent routedEvent, DateTime oldTime, DateTime newTime)
            : base(routedEvent)
        {
            OldTime = oldTime;
            NewTime = newTime;
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
