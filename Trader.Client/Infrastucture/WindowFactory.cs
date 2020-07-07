using System;
using Dragablz;
using Trader.Domain.Infrastucture;

namespace Trader.Client.Infrastucture
{
    public class WindowFactory : IWindowFactory
    {
        private readonly IObjectProvider _objectProvider;

        public WindowFactory(IObjectProvider objectProvider)
        {
            _objectProvider = objectProvider;
        }

        public MainWindow Create(bool showMenu = false)
        {
            MainWindow window = new MainWindow();
            WindowViewModel model = _objectProvider.Get<WindowViewModel>();
            if (showMenu) model.ShowMenu();

            window.DataContext = model;

            window.Closing += (sender, e) =>
                              {
                                  if (TabablzControl.GetIsClosingAsPartOfDragOperation(window)) return;

                                  IDisposable todispose = ((MainWindow)sender).DataContext as IDisposable;
                                  todispose?.Dispose();
                              };

            return window;
        }
    }
}