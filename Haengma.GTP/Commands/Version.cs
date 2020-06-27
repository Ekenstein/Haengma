namespace GTP.Commands
{
    public class Version : GtpCommand
    {
        public Version(int? id) : base(id, "version", new string[0])
        {
        }
    }
}
