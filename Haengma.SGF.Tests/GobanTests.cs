using Haengma.Core.Sgf;
using Haengma.Core.Utils;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Haengma.Core.Sgf.SgfProperty;
using static Haengma.Core.Utils.Collections;

namespace Haengma.SGF.Tests
{
    public class GobanTests
    {
        [Fact]
        public void SetBoardSize()
        {
            var tree = SgfGameTree.Empty.SetBoardSize(19);
            Assert.Equal(19, tree.GetBoardSize());
        }

        [Theory]
        [InlineData("(;HA[9]AB[dd][jd][pd][dj][jj][pj][dp][jp][pp])", 9, 19)]
        [InlineData("(;HA[8]AB[dd][jd][pd][dj][pj][dp][jp][pp])", 8, 19)]
        [InlineData("(;HA[7]AB[dd][pd][dj][jj][pj][dp][pp])", 7, 19)]
        [InlineData("(;HA[6]AB[dd][pd][dj][pj][dp][pp])", 6, 19)]
        [InlineData("(;HA[5]AB[dd][pd][jj][dp][pp])", 5, 19)]
        [InlineData("(;HA[4]AB[dd][pd][dp][pp])", 4, 19)]
        [InlineData("(;HA[3]AB[pd][dp][pp])", 3, 19)]
        [InlineData("(;HA[2]AB[pd][dp])", 2, 19)]
        [InlineData("(;HA[9]AB[dd][gd][jd][dg][gg][jg][dj][gj][jj])", 9, 13)]
        [InlineData("(;HA[8]AB[dd][gd][jd][dg][jg][dj][gj][jj])", 8, 13)]
        [InlineData("(;HA[7]AB[dd][jd][dg][gg][jg][dj][jj])", 7, 13)]
        [InlineData("(;HA[6]AB[dd][jd][dg][jg][dj][jj])", 6, 13)]
        [InlineData("(;HA[5]AB[dd][jd][gg][dj][jj])", 5, 13)]
        [InlineData("(;HA[4]AB[dd][jd][dj][jj])", 4, 13)]
        [InlineData("(;HA[3]AB[jd][dj][jj])", 3, 13)]
        [InlineData("(;HA[2]AB[jd][dj])", 2, 13)]
        [InlineData("(;HA[9]AB[cc][ec][gc][ce][ee][ge][cg][eg][gg])", 9, 9)]
        [InlineData("(;HA[8]AB[cc][ec][gc][ce][ge][cg][eg][gg])", 8, 9)]
        [InlineData("(;HA[7]AB[cc][gc][ce][ee][ge][cg][gg])", 7, 9)]
        [InlineData("(;HA[6]AB[cc][gc][ce][ge][cg][gg])", 6, 9)]
        [InlineData("(;HA[5]AB[cc][gc][ee][cg][gg])", 5, 9)]
        [InlineData("(;HA[4]AB[cc][gc][cg][gg])", 4, 9)]
        [InlineData("(;HA[3]AB[gc][cg][gg])", 3, 9)]
        [InlineData("(;HA[2]AB[gc][cg])", 2, 9)]
        public void PlaceFixedHandicap(string rawSgf, int handicap, int boardSize)
        {
            var fact = SgfReader.Parse(rawSgf);
            Assert.True(fact.Success);

            TestUtils.AssertProperty<HA>(fact.Value, ha => Assert.Equal(handicap, ha.Handicap));

            var actualTree = SgfGameTree.Empty.PlaceFixedHandicap(handicap, boardSize);
            TestUtils.AssertProperty<HA>(List.Of(actualTree), ha => Assert.Equal(handicap, ha.Handicap));

            var factStones = TestUtils.AssertAndGetPropertyValue<AB, IReadOnlySet<Point>>(fact.Value, ab => ab.Stones);
            var actualStones = TestUtils.AssertAndGetPropertyValue<AB, IReadOnlySet<Point>>(fact.Value, ab => ab.Stones);

            static IOrderedEnumerable<Point> OrderStones(IReadOnlySet<Point> stones) => stones.OrderBy(x => x.X).ThenBy(x => x.Y);

            Assert.Equal(OrderStones(factStones), OrderStones(actualStones));
        }

        [Theory]
        [InlineData(0, 19)]
        [InlineData(1, 19)]
        [InlineData(10, 19)]
        public void PlaceFixedHandicap_InvalidHandicap(int handicap, int boardSize)
        {
            Assert.True(handicap < 2 || handicap > Goban.GetMaxHandicap(boardSize));
            Assert.Throws<SgfException>(() => SgfGameTree.Empty.PlaceFixedHandicap(handicap, boardSize));
        }

        [Fact]
        public void CanNotPlaceFixedHandicapWhenBoardIsNotEmpty()
        {
            var tree = SgfGameTree.Empty.PlayMove(SgfColor.Black, new Move.Point(3, 3), 19);
            Assert.Throws<SgfException>(() => tree.PlaceFixedHandicap(4, 19));
        }

        [Fact]
        public void CapturingAStoneRemovesItFromTheBoard()
        {
            static IReadOnlySet<Point> Ponnuki(int x, int y) => SetOf(
                new Point(x - 1, y),
                new Point(x + 1, y),
                new Point(x, y - 1),
                new Point(x, y + 1)
            );

            var ponnuki = Ponnuki(4, 4);

            var tree = ponnuki.Aggregate(
                SgfGameTree.Empty.PlayMove(SgfColor.Black, new Move.Point(4, 4), 19),
                (tree, move) => tree.PlayMove(SgfColor.White, new Move.Point(move.X, move.Y), 19).PlayMove(SgfColor.Black, new Move.Pass(), 19)
            );

            var board = tree.GetBoard(19);
            Assert.Equal(ponnuki.Count, board.Stones.Count);
            Assert.Contains(board.Stones, stone => ponnuki.Contains(stone.Point));
            Assert.All(board.Stones, stone => Assert.Equal(SgfColor.White, stone.Color));
        }

        [Theory]
        [InlineData("(;AB[aa][ba]AW[ab][bb][ca])", SgfColor.White, 2)]
        [InlineData("(;B[dd];W[pp];B[fd];W[ee];B[ed];W[de];B[pd];W[fe];B[ge];W[gd];B[hd];W[gc];B[hc];W[fb];B[hb];W[cd];B[dc];W[cc];B[db];W[cb];B[gb];W[fc];B[fa];W[ea];B[ga];W[eb];B[da];W[ca];B[ec])", SgfColor.Black, 6)]
        public void CapturingAGroupRemovesTheGroupFromTheBoard(string sgf, SgfColor color, int captures)
        {
            var result = SgfReader.Parse(sgf);
            Assert.True(result.Success);

            Assert.All(result.Value, tree =>
            {
                var board = tree.GetBoard(19);
                Assert.Equal(captures, board.Captures[color]);
            });
        }

        [Fact]
        public void CanNotPlayAtOccupiedPoint()
        {
            var tree = SgfGameTree.Empty.PlayMove(SgfColor.Black, new Move.Point(3, 3), 19);
            Assert.Throws<SgfException>(() => tree.PlayMove(SgfColor.Black, new Move.Point(3, 3), 19));
        }

        [Fact]
        public void CanNotPlayAtPointThatWouldBeSuicide()
        {
            var sgf = "(;AW[cd][dd][de]AB[cc][dc][bd][ed][be][ee][cf][df])";
            var tree = SgfReader.Parse(sgf).Value.Single();
            Assert.Throws<SgfException>(() => tree.PlayMove(SgfColor.White, new Move.Point(3, 5), 19));
        }

        [Fact]
        public void AddingSamePropertyOverridesTheOldProperty()
        {
            var tree = SgfGameTree.Empty.SetBoardSize(19).SetBoardSize(9);
            var properties = tree.Sequence.SelectMany(x => x.Properties);
            Assert.Single(properties);
            Assert.Equal(9, tree.GetBoardSize());
        }

        [Fact]
        public void CaptureInASemeai()
        {
            var tree = SgfReader.Parse("(;AW[cn][bo][ap][bp][aq][cq][dq][ar][dr][er][as][ds]AB[cp][dp][ep][bq][eq][fq][gq][br][cr][fr][bs][es][fs])").Value.Single();

            var blackCaptures = tree.SetTurn(SgfColor.Black).PlayMove(SgfColor.Black, new Move.Point(3, 19), 19).GetBoard(19);
            Assert.Equal(5, blackCaptures.Captures[SgfColor.Black]);

            var whiteCaptures = tree.SetTurn(SgfColor.White).PlayMove(SgfColor.White, new Move.Point(3, 19), 19).GetBoard(19);
            Assert.Equal(4, whiteCaptures.Captures[SgfColor.White]);
        }
    }
}
