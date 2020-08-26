namespace Trader.Domain.Model
{
    public  struct Coords<T>
    {
     
        private T x;
        private T y;

        public T Y { get => y; set => y = value; }
        public T X { get => x; set => x = value; }
    }
}
