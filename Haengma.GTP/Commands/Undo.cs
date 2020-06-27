using System;
using System.Collections.Generic;
using System.Text;

namespace GTP.Commands
{
    /// <summary>
    /// The board configuration and the number of captured stones are reset to the state before the last move. 
    /// The last move is removed from the move history.
    /// </summary>
    public class Undo : GtpCommand
    {
        public Undo(int? id) : base(id, "undo", new string[0])
        {
        }

        public override string ToString() => "undo";
    }
}
