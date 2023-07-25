using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Trader.Client.Views
{
    public class ZoomboxViewer : ReactiveObject, IDisposable
    {
        public ZoomboxViewer()
        {
            Source = Path.Combine(CurrentDir, "Assets/abc.jpg");
        }
        public string CurrentDir => Directory.GetCurrentDirectory();

        public void Dispose()
        {
        }
        public string Source { get; set; }
    }
}
