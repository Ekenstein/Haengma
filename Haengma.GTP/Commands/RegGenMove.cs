namespace GTP.Commands
{
    /// <summary>
    /// This command differs from genmove in that it does not play the generated move. 
    /// It is also advisable to turn off any move randomization since that may cause meaningless 
    /// regression fluctuations.
    /// </summary>
    public class RegGenMove : GtpCommand
    {
        public Color Color { get; }

        public RegGenMove(int? id, Color color) : base(id, "reg_genmove", new [] { color.ToString() })
        {
            Color = color;
        }
    }
}
