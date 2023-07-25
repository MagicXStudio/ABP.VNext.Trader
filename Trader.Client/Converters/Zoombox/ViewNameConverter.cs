namespace Trader.Client.Converters.Zoombox
{
  public class ViewNameConverter : SimpleConverter
  {
    protected override object Convert( object value )
    {
      return value.ToString().Remove( 0, 13 );
    }
  }
}
