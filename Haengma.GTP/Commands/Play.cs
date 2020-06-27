namespace GTP.Commands
{
    /// <summary>
    /// A stone of the requested color is played at the requested vertex. The number of captured stones 
    /// is updated if needed and the move is added to the move history.
    /// </summary>
    public class Play : GtpCommand
    {
        public Color Color { get; }
        public Vertex Vertex { get; }

        public Play(int? id, Color color, Vertex vertex) : base(id, "play", new [] { color.ToString(), vertex.Serialize() })
        {
            Color = color;
            Vertex = vertex;
        }

        public override string ToString() => $"Play {Color} {Vertex}";
    }
}
