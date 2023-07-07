using System;
using System.Collections.Generic;
using TradeExample.Annotations;
using Trader.Domain.Model;

namespace Trader.Domain.Services
{
    public interface INearToMarketService
    {
        IObservable<IEnumerable<FileDetail>> Query([NotNull] Func<decimal> percentFromMarket);
    }
}