namespace Haengma.Core.Models
{
    public abstract record TimeSettings(int MainTimeInSeconds)
    {
        public sealed record ByoYomi(
            int MainTimeInSeconds,
            int ByoYomiPeriods,
            int ByoYomiSeconds
        ) : TimeSettings(MainTimeInSeconds);
    }
}
