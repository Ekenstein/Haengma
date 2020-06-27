namespace GTP.Commands
{
    /// <summary>
    /// Board size and komi are set to the values given in the sgf file. Board configuration, 
    /// number of captured stones, and move history are found by replaying the game record up to 
    /// the position before move_number or until the end if omitted.
    /// </summary>
    public class LoadSgf : GtpCommand
    {
        /// <summary>
        /// Name of an sgf file.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Optional move number.
        /// </summary>
        public int MoveNumber { get; }

        public LoadSgf(int? id, string fileName, int moveNumber) : base(id, "loadsgf", new [] { fileName, moveNumber.ToString() })
        {
            FileName = fileName;
            MoveNumber = moveNumber;
        }
    }
}
