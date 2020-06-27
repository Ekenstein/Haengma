namespace GTP.Commands
{
    public class FinalStatusList : GtpCommand
    {
        public StoneStatus Status { get; }

        public FinalStatusList(int? id, StoneStatus status) : base(id, "final_status_list", new [] { status.ToString() })
        {
            Status = status;
        }
    }
}
