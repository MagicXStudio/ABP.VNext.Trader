namespace Trader.Client.Converters.Zoombox
{
  public class ViewStackCountConverter : SimpleConverter
  {
    protected override object Convert( object value )
    {
      return ( ( int )value ) - 1;
    }
  }
}
