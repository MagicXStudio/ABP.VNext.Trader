using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Dragablz;
using DynamicData;
using DynamicData.Binding;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Trader.Domain.Infrastucture;

namespace Trader.Client.Infrastucture
{
    public class WindowViewModel : AbstractNotifyPropertyChanged, IDisposable
    {
        private readonly IObjectProvider _objectProvider;
        private readonly Command _showMenuCommand;
        private readonly IDisposable _cleanUp;
        private ViewContainer _selected;

        public ICommand MemoryCollectCommand { get; } = new Command(() =>
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        });


        public WindowViewModel(IObjectProvider objectProvider, IWindowFactory windowFactory)
        {
            _objectProvider = objectProvider;

            InterTabClient = new InterTabClient(windowFactory);
            _showMenuCommand = new Command(ShowMenu, () => Selected != null && !(Selected.Content is MenuItems));

            InitRabbitMQ();

            ShowInGitHubCommand = new Command(() => Process.Start(new ProcessStartInfo
            {
                FileName = "cmd",
                Arguments = "/c start https://github.com/wjkhappy14/Abp.VNext.Hello"
            }));

            LoginCommand = new Command(() =>
           {

           });

            IDisposable menuController = Views.ToObservableChangeSet()
                                        .Filter(vc => vc.Content is MenuItems)
                                        .Transform(vc => (MenuItems)vc.Content)
                                        .MergeMany(menuItem => menuItem.ItemCreated)
                                        .Subscribe(item =>
                                        {
                                            Views.Add(item);
                                            Selected = item;
                                        });


            _cleanUp = Disposable.Create(() =>
                                         {
                                             menuController.Dispose();
                                             foreach (IDisposable disposable in Views.Select(vc => vc.Content).OfType<IDisposable>())
                                                 disposable.Dispose();
                                         });


        }

        public void ShowMenu()
        {
            ViewContainer existing = Views.FirstOrDefault(vc => vc.Content is MenuItems);
            if (existing == null)
            {
                MenuItems newmenu = _objectProvider.Get<MenuItems>();
                ViewContainer newItem = new ViewContainer("菜单", newmenu);
                Views.Add(newItem);
                Selected = newItem;
            }
            else
            {
                Selected = existing;
            }
        }


        public ItemActionCallback ClosingTabItemHandler => ClosingTabItemHandlerImpl;

        private void ClosingTabItemHandlerImpl(ItemActionCallbackArgs<TabablzControl> args)
        {
            ViewContainer container = (ViewContainer)args.DragablzItem.DataContext;//.DataContext;
            if (container.Equals(Selected))
            {
                Selected = Views.FirstOrDefault(vc => vc != container);
            }
            var disposable = container.Content as IDisposable;
            disposable?.Dispose();
        }

        public ObservableCollection<ViewContainer> Views { get; } = new ObservableCollection<ViewContainer>();

        public ViewContainer Selected
        {
            get => _selected;
            set => SetAndRaise(ref _selected, value);
        }

        public IInterTabClient InterTabClient { get; }

        public ICommand ShowMenuCommand => _showMenuCommand;

        public Command ShowInGitHubCommand { get; }

        public Command LoginCommand { get; }

        public void Dispose()
        {
            _cleanUp.Dispose();
        }

        private void InitRabbitMQ()
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = "47.98.226.195",
                UserName = "admin",
                Password = "zxcvbnm",
                VirtualHost = "/"
            };
            IConnection connection = factory.CreateConnection();
            IModel channel = connection.CreateModel();
            channel.ExchangeDeclare("hello-exchange", "direct", true);
            channel.QueueDeclare("hello-abp", true, false, false, null);

            channel.QueueBind(
            queue: "hello-abp",
            exchange: "hello-exchange",
            routingKey: "System.Object"
            );
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
            string consumerTag = channel.BasicConsume(
                                          queue: "hello-abp",
                                          autoAck: false,
                                          consumer: consumer
                                          );

            consumer.Received += (model, args) =>
            {
                ReadOnlyMemory<byte> body = args.Body;
                string message = Encoding.UTF8.GetString(body.ToArray());
                MessageBox.Show(message);
                channel.BasicAck(args.DeliveryTag, false);
            };
        }
    }
}
