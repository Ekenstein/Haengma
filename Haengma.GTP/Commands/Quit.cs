namespace GTP.Commands
{
    /// <summary>
    /// The session is terminated and the connection is closed.
    /// </summary>
    public class Quit : GtpCommand
    {
        public Quit(int? id) : base(id, "quit", new string[0])
        {
        }
    }
}
