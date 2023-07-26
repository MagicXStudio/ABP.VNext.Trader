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
    ///https://learn.microsoft.com/zh-cn/dotnet/api/system.windows.media.imaging.writeablebitmap?view=windowsdesktop-7.0
    /// </summary>
    public partial class WriteableBitmapView : UserControl
    {
        public WriteableBitmapView()
        {
            InitializeComponent();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            WriteableBitmap bmp = new WriteableBitmap(1, 1, 1, 1, new PixelFormat(), new BitmapPalette(null));

        }
    }
}
