namespace GTP.Commands
{
    /// <summary>
    /// The engine may draw the board as it likes. It is, however, required to place the 
    /// coordinates as described in section 2.11. This command is only intended to help 
    /// humans with debugging and the output should never need to be parsed by another program.
    /// </summary>
    public class ShowBoard : GtpCommand
    {
        public ShowBoard(int? id) : base(id, "showboard", new string[0])
        {
        }
    }
}
