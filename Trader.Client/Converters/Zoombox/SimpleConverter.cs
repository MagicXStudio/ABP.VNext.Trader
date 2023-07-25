using System;
using System.Windows.Data;

namespace Trader.Client.Converters.Zoombox
{
  public abstract class SimpleConverter : IValueConverter
  {
    protected abstract object Convert( object value );

    public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
    {
      return this.Convert( value );
    }

    public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
    {
      throw new NotImplementedException();
    }
  }
}
