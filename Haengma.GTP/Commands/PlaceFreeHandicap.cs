namespace GTP.Commands
{
    /// <summary>
    /// Handicap stones are placed on the board on the vertices the engine prefers. See also section 4.1.2.
    /// </summary>
    public class PlaceFreeHandicap : GtpCommand
    {
        public PlaceFreeHandicap(int? id, int numberOfStones) : base(id, "place_free_handicap", new [] { numberOfStones.ToString() })
        {
            NumberOfStones = numberOfStones;
        }

        /// <summary>
        /// Number of handicap stones.
        /// </summary>
        public int NumberOfStones { get; }
    }
}
