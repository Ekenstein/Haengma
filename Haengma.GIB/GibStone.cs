namespace Haengma.GIB
{
    public class GibStone
    {
        private readonly string[] _values;

        internal GibStone(string[] value)
        {
            _values = value;
        }

        public (int x, int y) Coordinates
        {
            get
            {
                var x = int.Parse(_values[4]);
                var y = int.Parse(_values[5]);

                return (x, y);
            }
        }

        public GibColor Color => _values[3] == "1"
            ? GibColor.Black
            : GibColor.White;

        public int MoveNumber => int.Parse(_values[2]);
        public int Variation => int.Parse(_values[1]);

        public override string ToString() => string.Join(" ", _values);
    }
}
