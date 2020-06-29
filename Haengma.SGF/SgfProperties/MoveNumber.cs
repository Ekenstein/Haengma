using Haengma.SGF.ValueTypes;

namespace Haengma.SGF.SgfProperties
{
    public class MoveNumber : SgfProperty
    {
        public int Number { get; }

        public MoveNumber(int number) : base("MN", new SgfNumber(number))
        {
            Number = number;
        }
    }
}
