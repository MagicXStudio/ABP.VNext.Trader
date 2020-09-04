namespace Trader.Domain.Algorithms
{
    public static class Basic
    {
        /// <summary>
        ///  -128>>(n-1)
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private static byte MaskHigh(int n) => (byte)(sbyte.MinValue >> (n - 1));
    }
}
