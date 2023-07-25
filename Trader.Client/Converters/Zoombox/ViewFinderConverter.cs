namespace Trader.Client.Converters.Zoombox
{
  public class ViewFinderConverter : SimpleConverter
  {
    protected override object Convert( object value )
    {
      return ( value != null ) ? value.GetType().Name : null;
    }
  }
}
