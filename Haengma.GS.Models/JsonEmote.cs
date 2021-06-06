namespace Haengma.GS.Models
{
    public record JsonEmoteMessage(JsonColor Sender, JsonEmote Emote);

    public enum JsonEmote
    {
        Greetings,
        Bye,
        Mistake,
        Impressed,
        Thanks
    }
}
