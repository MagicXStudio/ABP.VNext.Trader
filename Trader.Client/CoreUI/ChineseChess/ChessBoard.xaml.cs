using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Trader.Client.CoreUI.ChineseChess
{
    /// <summary>
    /// ChessBoard.xaml 的交互逻辑
    /// </summary>

    public partial class ChessBoard : UserControl
    {
        public ChessBoard()
        {
            InitializeComponent();

        }
        private bool _isDown;
        private bool _isDragging;
        private Canvas BoardCanvas { get; set; }
        private UIElement _originalElement;
        private double _originalLeft;
        private double _originalTop;
        private CircleAdorner _overlayElement;
        private Point _startPoint;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            BoardCanvas = new Canvas();
            foreach (int i in Enumerable.Range(0, 10))
            {
                ChessPiece piece = new ChessPiece($"{i}", i);
                piece.Height = piece.Width = 120;

                Canvas.SetTop(piece, i * piece.Height);
                Canvas.SetLeft(piece, i * piece.Width);
                BoardCanvas.Children.Add(piece);
            }
            var tb = new TextBox { Text = "PV=计划值、EV=挣值、AC=实际成本" };
            Canvas.SetTop(tb, 100);
            Canvas.SetLeft(tb, 100);
            BoardCanvas.Children.Add(tb);

            BoardCanvas.PreviewMouseLeftButtonDown += MyCanvas_PreviewMouseLeftButtonDown;
            BoardCanvas.PreviewMouseMove += MyCanvas_PreviewMouseMove;
            BoardCanvas.PreviewMouseLeftButtonUp += MyCanvas_PreviewMouseLeftButtonUp;
            PreviewKeyDown += window1_PreviewKeyDown;

            myStackPanel.Children.Add(BoardCanvas);
        }

        private void window1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && _isDragging)
            {
                DragFinished(true);
            }
        }

        private void MyCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDown)
            {
                DragFinished(false);
                e.Handled = true;
            }
        }

        private void DragFinished(bool cancelled)
        {
            Mouse.Capture(null);
            if (_isDragging)
            {
                AdornerLayer.GetAdornerLayer(_overlayElement.AdornedElement).Remove(_overlayElement);

                if (cancelled == false)
                {
                    Canvas.SetTop(_originalElement, _originalTop + _overlayElement.TopOffset);
                    Canvas.SetLeft(_originalElement, _originalLeft + _overlayElement.LeftOffset);
                }
                _overlayElement = null;
            }
            _isDragging = false;
            _isDown = false;
        }

        private void MyCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDown)
            {
                if ((_isDragging == false) &&
                    ((Math.Abs(e.GetPosition(BoardCanvas).X - _startPoint.X) >
                      SystemParameters.MinimumHorizontalDragDistance) ||
                     (Math.Abs(e.GetPosition(BoardCanvas).Y - _startPoint.Y) >
                      SystemParameters.MinimumVerticalDragDistance)))
                {
                    DragStarted();
                }
                if (_isDragging)
                {
                    DragMoved();
                }
            }
        }

        private void DragStarted()
        {
            _isDragging = true;
            _originalLeft = Canvas.GetLeft(_originalElement);
            _originalTop = Canvas.GetTop(_originalElement);

            _overlayElement = new CircleAdorner(_originalElement);
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(_originalElement);
            layer.Add(_overlayElement);
        }

        private void DragMoved()
        {
            Point currentPosition = Mouse.GetPosition(BoardCanvas);

            _overlayElement.LeftOffset = currentPosition.X - _startPoint.X;
            _overlayElement.TopOffset = currentPosition.Y - _startPoint.Y;
        }

        private void MyCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source == BoardCanvas)
            {
            }
            else
            {
                _isDown = true;
                _startPoint = e.GetPosition(BoardCanvas);
                _originalElement = e.Source as UIElement;
                BoardCanvas.CaptureMouse();
                e.Handled = true;
            }
        }
    }
}
