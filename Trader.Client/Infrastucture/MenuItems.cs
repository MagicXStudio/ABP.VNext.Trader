using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
                    "A basic example,  filter and bind.",
                    () => Open<LiveTradesViewer>("实时动态")),

                new MenuItem("我的日程",
                     "Dynamic filtevalues.",
                     () => Open<NearToMarketViewer>("我的日程")),

                new MenuItem("联系人",
                       "Group tradouping.",
                        () => Open<TradesByPercentViewer>("联系人")),

                new MenuItem("Trades By hh:mm",
                       "Group items by ti time passes",
                        () => Open<TradesByTimeViewer>("Trades By hh:mm")),

                new MenuItem("最近交易",
                    "Operator whic last minute.",
                    () => Open<RecentTradesViewer>("Recent Trades")),

                new MenuItem("Trading持仓",
                       "Calculate overall  and aggregate totals",
                        () => Open<PositionsViewer>("Trading Positions")),

                  new MenuItem("ABP vNext",
                       "ABP vNext",
                        () => Open<AbpViewer>("ABP vNext")),

                     new MenuItem("聊天",
                       "Chat",
                        () => Open<ChatViewer>("聊天"),
                        MenuCategory.ReactiveUi),

                        new MenuItem("身份认证",
                       "Login",
                        () => Open<LoginViewer>("身份认证"),
                        MenuCategory.ReactiveUi),

                      new MenuItem("Ids4",
                       "IdentityServer4",
                        () => Open<IdentityServer4Viewer>("Ids4"),
                        MenuCategory.ReactiveUi),

                  new MenuItem("Paged Data",
                    "An adva data",
                    () => Open<PagedDataViewer>("Paged Data"),new []
                        {
                            new Link("Blog","Sort Filter a Data", " http://dynamic-data.org/2015/04/22/dynamically-sort-filter-and-page-data/"),
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

            T content = _objectProvider.Get<T>();
            RxUiHostViewModel rxuiContent = new RxUiHostViewModel(content);

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