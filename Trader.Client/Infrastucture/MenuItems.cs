using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using DynamicData.Binding;
using ReactiveUI;
using Trader.Client.Views;
using Trader.Domain.Infrastucture;

namespace Trader.Client.Infrastucture
{
    public enum MenuCategory
    {
        ReactiveUi,
        DynamicData
    }


    public class MenuItems : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IObjectProvider _objectProvider;
        private readonly IEnumerable<MenuItem> _menuItems;
        private readonly ISubject<ViewContainer> _viewCreatedSubject = new Subject<ViewContainer>();

        private readonly IDisposable _cleanUp;
        private bool _showLinks = false;
        private MenuCategory _category = MenuCategory.DynamicData;
        private IEnumerable<MenuItem> _items;


        public MenuItems(ILogger logger, IObjectProvider objectProvider)
        {
            _logger = logger;
            _objectProvider = objectProvider;

            _menuItems = new List<MenuItem>
            {
                new MenuItem("实时动态",
                    "A basic example, illustrating how to connect to a stream, inject a user filter and bind.",
                    () => Open<LiveTradesViewer>("实时动态")),

                new MenuItem("Near to Market",
                     "Dynamic filtering of calculated values.",
                     () => Open<NearToMarketViewer>("Near to Market")),

                new MenuItem("Trades By %",
                       "Group trades by a constantly changing calculated value. With automatic regrouping.",
                        () => Open<TradesByPercentViewer>("Trades By % Diff")),

                new MenuItem("Trades By hh:mm",
                       "Group items by time with automatic regrouping as time passes",
                        () => Open<TradesByTimeViewer>("Trades By hh:mm")),

                new MenuItem("最近交易",
                    "Operator whic last minute.",
                    () => Open<RecentTradesViewer>("Recent Trades")),


                new MenuItem("Trading持仓",
                       "Calculate overall position for each currency pair and aggregate totals",
                        () => Open<PositionsViewer>("Trading Positions")),

                  new MenuItem("ABP vNext",
                       "ABP vNext",
                        () => Open<AbpViewer>("ABP vNext")),

                      new MenuItem("IdentityServer4",
                       "IdentityServer4",
                        () => Open<IdentityServer4Viewer>("IdentityServer4")),

                  new MenuItem("Paged Data",
                    "An advanced example of how to page data",
                    () => Open<PagedDataViewer>("Paged Data"),new []
                        {
                            new Link("Blog","Sort Filter and Page Data", " http://dynamic-data.org/2015/04/22/dynamically-sort-filter-and-page-data/"),
                        })
            };

            IDisposable filterApplier = this.WhenValueChanged(t => t.Category)
                .Subscribe(value =>
                {
                    Items = _menuItems.Where(menu => menu.Category == value).ToArray();
                });

            _cleanUp = Disposable.Create(() =>
            {
                _viewCreatedSubject.OnCompleted();
                filterApplier.Dispose();
            });
        }

        private void Open<T>(string title)
        {

            _logger.Debug("Opening '{0}'", title);

            T content = _objectProvider.Get<T>();
            _viewCreatedSubject.OnNext(new ViewContainer(title, content));
            _logger.Debug("--Opened '{0}'", title);
        }

        private void OpenRxUI<T>(string title)
            where T : ReactiveObject
        {
            _logger.Debug("Opening '{0}'", title);

            var content = _objectProvider.Get<T>();
            var rxuiContent = new RxUiHostViewModel(content);

            _viewCreatedSubject.OnNext(new ViewContainer(title, rxuiContent));
            _logger.Debug("--Opened '{0}'", title);
        }

        public MenuCategory Category
        {
            get => _category;
            set => SetAndRaise(ref _category, value);
        }

        public IEnumerable<MenuItem> Items
        {
            get => _items;
            set => SetAndRaise(ref _items, value);
        }

        public bool ShowLinks
        {
            get => _showLinks;
            set => SetAndRaise(ref _showLinks, value);
        }

        public IObservable<ViewContainer> ItemCreated => _viewCreatedSubject.AsObservable();

        public void Dispose()
        {
            _cleanUp.Dispose();
        }
    }
}