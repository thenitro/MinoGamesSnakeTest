namespace TymurKoshel.Snake.Controllers.Base
{
    public class Index
    {
        public int X;
        public int Y;

        public Index(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"[Index {X} {Y}]";
        }
    }
}