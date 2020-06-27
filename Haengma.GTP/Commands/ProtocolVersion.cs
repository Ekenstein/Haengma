namespace GTP.Commands
{
    public class ProtocolVersion : GtpCommand
    {
        public ProtocolVersion(int? id) : base(id, "protocol_version", new string[0])
        {
        }
    }
}
