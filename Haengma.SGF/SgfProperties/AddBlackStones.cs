using Haengma.SGF.ValueTypes;
using System.Linq;

namespace Haengma.SGF.SgfProperties
{
    public class AddBlackStones : SgfProperty
    {
        public static SgfPoint[] Handicap(int handicap)
        {
            if (handicap < 2)
            {
                return new SgfPoint[0];
            }

            return handicap switch
            {
                2 => new[]
                {
                    new SgfPoint(3, 15),
                    new SgfPoint(15, 3)
                },
                3 => Handicap(2).Concat(new[] { new SgfPoint(15, 15) }).ToArray(),
                4 => Handicap(3).Concat(new[] { new SgfPoint(3, 3) }).ToArray(),
                5 => Handicap(4).Concat(new[] { new SgfPoint(9, 9) }).ToArray(),
                6 => Handicap(4).Concat(new[] { new SgfPoint(3, 9), new SgfPoint(15, 9) }).ToArray(),
                7 => Handicap(6).Concat(new[] { new SgfPoint(9, 9) }).ToArray(),
                8 => Handicap(6).Concat(new[] { new SgfPoint(9, 3), new SgfPoint(9, 15)}).ToArray(),
                9 => Handicap(8).Concat(new[] { new SgfPoint(9, 9) }).ToArray(),
                _ => new SgfPoint[0]
            };
        }

        public AddBlackStones(params SgfPoint[] stones) : base("AB", stones)
        {
        }
    }
}
