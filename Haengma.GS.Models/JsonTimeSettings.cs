namespace Haengma.GS.Models
{
    public enum JsonTimeSettingType { ByoYomi }
    public record JsonTimeSettings(JsonTimeSettingType Type, int MainTimeInSeconds, int ByoYomiPeriods, int ByoYomiSeconds);
}
