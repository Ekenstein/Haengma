namespace GTP.Commands
{
    public class KnownCommand : GtpCommand
    {
        public string CommandName { get; }

        public KnownCommand(int? id, string command) : base(id, "known_command", new [] { command })
        {
            CommandName = command;
        }
    }
}
