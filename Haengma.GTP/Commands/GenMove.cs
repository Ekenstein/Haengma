namespace GTP.Commands
{
    /// <summary>
    /// A stone of the requested color is played where the engine chooses. The number of captured stones is updated 
    /// if needed and the move is added to the move history.
    /// </summary>
    public class GenMove : GtpCommand
    {
        public Color Color { get; }

        public GenMove(int? id, Color color) : base(id, "genmove", new [] { color.ToString() })
        {
            Color = color;
        }

        public override string ToString() => $"Generate move for {Color}";
    }
}
