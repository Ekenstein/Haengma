namespace Haengma.Core
{
    public interface IIdGenerator<TKey>
    {
        TKey Generate();
    }
}
