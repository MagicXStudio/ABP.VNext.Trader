using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Trader.Client.Views
{
    /// <summary>
    /// ZoomboxView.xaml 的交互逻辑
    /// </summary>
    public partial class ZoomboxView : UserControl
    {
        public ZoomboxView()
        {
            InitializeComponent();
        }

        private void AdjustAnimationDuration(object sender, RoutedPropertyChangedEventArgs<double> e)
        { 
        
        }
        private void CoerceAnimationRatios(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        
        }
    }
}
