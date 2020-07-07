using DynamicData.Binding;
using System.Windows;
using Trader.Client.Infrastucture;

namespace Trader.Client.Views
{
    public class AbpViewer : AbstractNotifyPropertyChanged
    {
        public AbpViewer()
        {

        }

        public Command HelloCommand => new Command(() =>
                                                 {
                                                     MessageBox.Show("Hello ABP vNext");
                                                 });
    }
}
