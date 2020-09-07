namespace Trader.Domain.Model
{

    ///https://docs.microsoft.com/zh-cn/dotnet/csharp/language-reference/operators/
    /// <summary>
    ///   ~、&、|、^、<< 、 >>、 %、/、*、
    /// </summary>
    public static class Operators
    {

        /// <summary>
        /// 二进制     &（逻辑 AND）、|（逻辑 OR）, ^（逻辑异或）
        /// </summary>
        public enum Logical
        {
            AND,//&
            XOR,//^
            OR//|
        }


        /// <summary>
        /// <<,>>
        /// </summary>
        public enum Shift
        {
            Left,
            Right
        }

        /// <summary>
        /// 按位求补 ~
        /// </summary>
        public enum Bitwise
        {


        }

        /// <summary>
        /// *, /, %, +,-
        /// </summary>

        public enum Arithmetic
        {
            Plus,//+
            Minus,//-
            Multiply
        }
    }
}
