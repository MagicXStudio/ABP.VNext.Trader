using System;
using System.Collections.Generic;
using System.Drawing;
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
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;
using Pen = System.Windows.Media.Pen;

namespace Trader.Client.Views
{
    /// <summary>
    /// PaintView.xaml 的交互逻辑
    /// </summary>
    public partial class PaintView : UserControl
    {
        public PaintView()
        {
            InitializeComponent();
            Graphics = new DrawingGroup();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var ctx = Graphics.Open();
            Pen pen = new Pen(Brushes.Red, 13);
            pen.Freeze();
            drawingContext.DrawLine(pen, new System.Windows.Point(100, 200), new System.Windows.Point(400, 600));
            Rect rect = new Rect(new System.Windows.Point(160, 100), new System.Windows.Size(320, 80));
            drawingContext.DrawRectangle(Brushes.Yellow, pen, rect);
            ctx.Close();
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var ctx = Graphics.Open();
            Pen pen = new Pen(Brushes.Red, 13);
            System.Windows.Point position = e.GetPosition(this);
            Rect rect = new Rect(new System.Windows.Point(160, 100), new System.Windows.Size(320, 80));
            ctx.DrawRectangle(Brushes.Yellow, pen, rect);
            ctx.Close();
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
        }

        private DrawingGroup Graphics { get; set; }
    }
}
