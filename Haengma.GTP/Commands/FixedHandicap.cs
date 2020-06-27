using System;
using System.Collections.Generic;
using System.Text;

namespace GTP.Commands
{
    /// <summary>
    /// Handicap stones are placed on the board according to the specification in section 4.1.1.
    /// </summary>
    public class FixedHandicap : GtpCommand
    {
        public int NumberOfStones { get; }

        public FixedHandicap(int? id, int numberOfStones) : base(id, "fixed_handicap", new [] { numberOfStones.ToString() })
        {
            NumberOfStones = numberOfStones;
        }

        public override string ToString() => $"Fixed handicap {NumberOfStones}";
    }
}
