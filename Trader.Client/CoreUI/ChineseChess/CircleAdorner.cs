using System;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;

namespace Trader.Client.CoreUI.ChineseChess
{
    public class CircleAdorner : Adorner
    {
        private readonly Rectangle _child;
        private double _leftOffset;
        private double _topOffset;
        public CircleAdorner(UIElement adornedElement) : base(adornedElement)
        {
            VisualBrush brush = new VisualBrush(adornedElement);

            _child = new Rectangle
            {
                Width = adornedElement.RenderSize.Width,
                Height = adornedElement.RenderSize.Height
            };
            DoubleAnimation animation = new DoubleAnimation(0.3, 1, new Duration(TimeSpan.FromSeconds(1)))
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };
            brush.BeginAnimation(Brush.OpacityProperty, animation);

            _child.Fill = brush;
        }

        protected override int VisualChildrenCount => 1;

        public double LeftOffset
        {
            get { return _leftOffset; }
            set
            {
                _leftOffset = value;
                UpdatePosition();
            }
        }

        public double TopOffset
        {
            get { return _topOffset; }
            set
            {
                _topOffset = value;
                UpdatePosition();
            }
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect adornedElementRect = new Rect(AdornedElement.DesiredSize);
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.Green) { Opacity = 0.2 };
            Pen renderPen = new Pen(new SolidColorBrush(Colors.Navy), 1.5);
            const double renderRadius = 5.0;

            drawingContext.DrawRectangle(renderBrush, renderPen, adornedElementRect);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            _child.Measure(constraint);
            return _child.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _child.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index) => _child;

        private void UpdatePosition()
        {
            AdornerLayer adornerLayer = Parent as AdornerLayer;
            adornerLayer?.Update(AdornedElement);
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            GeneralTransformGroup result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(_leftOffset, _topOffset));
            return result;
        }
    }
}
