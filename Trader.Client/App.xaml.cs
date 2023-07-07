using System;
using System.Reactive.PlatformServices;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using Abp.VNext.Hello.XNetty;
using DotNetty.Transport.Bootstrapping;
using Microsoft.Extensions.Configuration;
using ReactiveUI;
using StructureMap;
using Trader.Client.Infrastucture;
using Trader.Client.Views;
using Trader.Domain.Infrastucture;
using Trader.Domain.Services;

namespace Trader.Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Application" /> class.</summary>
        /// <exception cref="T:System.InvalidOperationException">More than one instance of the <see cref="T:System.Windows.Application" /> class is created per <see cref="T:System.AppDomain" />.</exception>
        public App()
        {
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
            ShutdownMode = ShutdownMode.OnLastWindowClose;
        }

        private void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Console.WriteLine(args.LoadedAssembly);
        }

        /// <summary>Raises the <see cref="E:System.Windows.Application.Startup" /> event.</summary>
        /// <param name="e">A <see cref="T:System.Windows.StartupEventArgs" /> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            Container container = new Container(x => x.AddRegistry<AppRegistry>());
            WindowFactory factory = container.GetInstance<WindowFactory>();
            MainWindow window = factory.Create(true);
            container.Configure(x => x.For<Dispatcher>().Add(window.Dispatcher));

            IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
            container.Configure(x => x.For<IConfigurationRoot>().Add(configuration));

            //configure dependency resolver for RxUI / Splat
            ReactiveUIDependencyResolver resolver = new ReactiveUIDependencyResolver(container);
            resolver.Register(() => new LogEntryView(), typeof(IViewFor<LogEntryViewer>));
            resolver.Register(() => new PhotoView(), typeof(IViewFor<PhotoViewer>));

            //Locator.Current = resolver;
            //RxApp.SupportsRangeNotifications = false;
            //run start up jobs
            container.GetInstance<TradePriceUpdateJob>();
            container.GetInstance<ILogEntryService>();

            TaskAwaiter<Bootstrap> w = ClientBootstrap.Client.InitBootstrapAsync().GetAwaiter();
            window.Show();
            base.OnStartup(e);
        }
    }
}
