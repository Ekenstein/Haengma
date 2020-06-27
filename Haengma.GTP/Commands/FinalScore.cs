namespace GTP.Commands
{
    public class FinalScore : GtpCommand
    {
        public FinalScore(int? id) : base(id, "final_score", new string[0])
        {
        }

        public override string ToString() => "Final score";
    }
}
