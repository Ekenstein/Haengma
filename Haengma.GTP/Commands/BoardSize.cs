namespace GTP.Commands
{
    /// <summary>
    /// The board size is changed. The board configuration, number of captured stones, 
    /// and move history become arbitrary.
    /// </summary>
    public class BoardSize : GtpCommand
    {
        public int Size { get; }

        public BoardSize(int? id, int size) : base(id, "boardsize", new [] { size.ToString() })
        {
            Size = size;
        }

        public override string ToString() => $"Board size {Size}";
    }
}
