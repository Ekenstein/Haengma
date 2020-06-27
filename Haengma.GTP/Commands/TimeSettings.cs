namespace GTP.Commands
{
    /// <summary>
    /// The time settings are changed.
    /// </summary>
    public class TimeSettings : GtpCommand
    {
        /// <summary>
        /// Main time measured in seconds.
        /// </summary>
        public int MainTime { get; }

        /// <summary>
        /// Byo yomi time measured in seconds.
        /// </summary>
        public int ByoYomiTime { get; }

        /// <summary>
        /// Number of stones per byo yomi period.
        /// </summary>
        public int ByoYomiStones { get; }

        public TimeSettings(int? id, int mainTime, int byoYomiTime, int byoYomiStones) : 
            base(id, "time_settings", new [] { mainTime.ToString(), byoYomiTime.ToString(), byoYomiStones.ToString() })
        {
            MainTime = mainTime;
            ByoYomiStones = byoYomiStones;
            ByoYomiTime = byoYomiTime;
        }

        public override string ToString() => "Time settings";
    }
}
