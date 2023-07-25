using System.Windows;

namespace Trader.Client.Converters.Zoombox
{
  public class RectConverter : SimpleConverter
  {
    protected override object Convert( object value )
    {
      return string.Format( "({0}),({1})",
        PointConverter.ConvertPoint( ( ( Rect )value ).TopLeft ),
        PointConverter.ConvertPoint( ( ( Rect )value ).BottomRight ) );
    }
  }
}
