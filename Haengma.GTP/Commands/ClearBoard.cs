namespace GTP.Commands
{
    /// <summary>
    /// The board is cleared, the number of captured stones is reset to zero for 
    /// both colors and the move history is reset to empty.
    /// </summary>
    public class ClearBoard : GtpCommand
    {
        public ClearBoard(int? id) : base(id, "clear_board", new string[0])
        {
        }

        public override string ToString() => "Clear board";
    }
}
