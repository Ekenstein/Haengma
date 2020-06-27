namespace Haengma.SGF.ValueTypes
{
    public class SgfPoint : SgfValue
    {
        public int X { get; }
        public int Y { get; }

        public override string Value => ToSgfPoint(X, Y);

        public SgfPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        private static string ToSgfPoint(int x, int y)
        {
            return $"{IntToChar(x)}{IntToChar(y)}";
        }

        private static char IntToChar(int x) => (char)(x + 'a');
        public static int CharToInt(char c) => c - 'a';
    }
}
