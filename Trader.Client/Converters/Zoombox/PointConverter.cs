using System;
using System.Windows;

namespace Trader.Client.Converters.Zoombox
{
  public class PointConverter : SimpleConverter
  {
    protected override object Convert( object value )
    {
      return PointConverter.ConvertPoint( ( Point )value );
    }

    public static string ConvertPoint( Point point )
    {
      return string.Format( "{0},{1}",
        Math.Round( point.X ), Math.Round( point.Y ) );
    }
  }
}
