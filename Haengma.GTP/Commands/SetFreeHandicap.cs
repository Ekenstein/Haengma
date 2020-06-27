using System.Linq;

namespace GTP.Commands
{
    public class SetFreeHandicap : GtpCommand
    {
        public Vertice[] Vertices { get; }

        public SetFreeHandicap(int? id, Vertice[] vertices) : base(id, "set_free_handicap", vertices.Select(v => v.Serialize()).ToArray())
        {
            Vertices = vertices;
        }

        public override string ToString() => $"Set free handicap {string.Join(" ", Vertices)}";
    }
}
