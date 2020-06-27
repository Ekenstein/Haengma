using Monadicsh;
using Monadicsh.Extensions;

namespace GTP
{
    public class Vertex : Either<Pass, Vertice>
    {
        public Vertex(Pass left) : base(left)
        {
        }

        public Vertex(Vertice right) : base(right)
        {
        }

        public override string ToString() => this.MapEither(l => l.ToString(), r => r.ToString());

        public string Serialize() => this.MapEither(l => l.Serialize(), r => r.Serialize());
    }

    public struct Vertice
    {
        public int X { get; }
        public int Y { get; }

        public Vertice(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => $"{IntToChar(X)}{Y}";

        public string Serialize() => $"{IntToChar(X)}{Y}";

        private static char IntToChar(int x) => (char)(x + 'a');
        public static int CharToInt(char c) => c - 'a';
    }

    public struct Pass 
    {
        public override string ToString() => "Pass";

        public string Serialize() => "pass";
    }
}
