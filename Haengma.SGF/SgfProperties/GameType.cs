using Haengma.SGF.ValueTypes;

namespace Haengma.SGF.SgfProperties
{
    public class GameType : SgfProperty
    {
        public static GameType Go => new GameType(1);

        private GameType(int type) : base("GM", new SgfNumber(type))
        {
        }
    }
}
