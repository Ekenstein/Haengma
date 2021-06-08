using Haengma.Core.Sgf;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Haengma.Core.Sgf.SgfProperty;
using static Haengma.Core.Utils.Collections;
using static Haengma.Tests.SgfAssert;
using static Xunit.Assert;

namespace Haengma.Tests.Haengma.Core.Sgf
{
    public class GobanTests
    {
        [Fact]
        public void SetBoardSize()
        {
            var tree = SgfGameTree.Empty.SetBoardSize(19);
            Equal(19, tree.GetBoardSize());
            ContainsSingleProperty<SZ>(tree, x => Equal(19, x.Size));
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
            var actualTree = SgfGameTree.Empty.PlaceFixedHandicap(handicap, boardSize);
            ContainsProperty<HA>(actualTree, x => Equal(handicap, x.Handicap));


            var result = SgfReader.Parse(rawSgf);
            ParseSuccess(result, rawSgf, trees => {
                SingleSgfTree(trees, tree =>
                {
                    ContainsProperty<HA>(tree, x => Equal(handicap, x.Handicap));
                    ContainsProperty<AB>(tree, expected =>
                    {
                        ContainsProperty<AB>(actualTree, actual =>
                        {
                            True(expected.Stones.SetEquals(actual.Stones));
                        });
                    });
                });
            });
        }

        [Theory]
        [InlineData(0, 19)]
        [InlineData(1, 19)]
        [InlineData(10, 19)]
        public void PlaceFixedHandicap_InvalidHandicap(int handicap, int boardSize)
        {
            True(handicap < 2 || handicap > Goban.GetMaxHandicap(boardSize));
            Throws<SgfException>(() => SgfGameTree.Empty.PlaceFixedHandicap(handicap, boardSize));
        }

        [Fact]
        public void CanNotPlaceFixedHandicapWhenBoardIsNotEmpty()
        {
            var tree = SgfGameTree.Empty.PlayMove(SgfColor.Black, new Move.Point(3, 3), 19);
            Throws<SgfException>(() => tree.PlaceFixedHandicap(4, 19));
        }

        [Fact]
        public void CapturingAStoneRemovesItFromTheBoard()
        {
            static IReadOnlySet<SgfPoint> Ponnuki(int x, int y) => SetOf(
                new SgfPoint(x - 1, y),
                new SgfPoint(x + 1, y),
                new SgfPoint(x, y - 1),
                new SgfPoint(x, y + 1)
            );

            var ponnuki = Ponnuki(4, 4);

            var tree = ponnuki.Aggregate(
                SgfGameTree.Empty.PlayMove(SgfColor.Black, new Move.Point(4, 4), 19),
                (tree, move) => tree.PlayMove(SgfColor.White, new Move.Point(move.X, move.Y), 19).PlayMove(SgfColor.Black, new Move.Pass(), 19)
            );

            var board = tree.GetBoard(19);
            Equal(ponnuki.Count, board.Stones.Count);
            Contains(board.Stones, stone => ponnuki.Contains(stone.Point));
            All(board.Stones, stone => Equal(SgfColor.White, stone.Color));
        }

        [Theory]
        [InlineData("(;AB[aa][ba]AW[ab][bb][ca])", SgfColor.White, 2)]
        [InlineData("(;B[dd];W[pp];B[fd];W[ee];B[ed];W[de];B[pd];W[fe];B[ge];W[gd];B[hd];W[gc];B[hc];W[fb];B[hb];W[cd];B[dc];W[cc];B[db];W[cb];B[gb];W[fc];B[fa];W[ea];B[ga];W[eb];B[da];W[ca];B[ec])", SgfColor.Black, 6)]
        public void CapturingAGroupRemovesTheGroupFromTheBoard(string sgf, SgfColor color, int captures)
        {
            var result = SgfReader.Parse(sgf);
            True(result.Success);

            All(result.Value, tree =>
            {
                var board = tree.GetBoard(19);
                Equal(captures, board.Captures[color]);
            });
        }

        [Fact]
        public void CanNotPlayAtOccupiedPoint()
        {
            var tree = SgfGameTree.Empty.PlayMove(SgfColor.Black, new Move.Point(3, 3), 19);
            Throws<SgfException>(() => tree.PlayMove(SgfColor.Black, new Move.Point(3, 3), 19));
        }

        [Fact]
        public void CanNotPlayAtPointThatWouldBeSuicide()
        {
            var sgf = "(;AW[cd][dd][de]AB[cc][dc][bd][ed][be][ee][cf][df])";
            var tree = SgfReader.Parse(sgf).Value.Single();
            Throws<SgfException>(() => tree.PlayMove(SgfColor.White, new Move.Point(3, 5), 19));
        }

        [Fact]
        public void AddingSamePropertyOverridesTheOldProperty()
        {
            var tree = SgfGameTree.Empty.SetBoardSize(19).SetBoardSize(9);
            ContainsSingleProperty<SZ>(tree, x =>
            {
                Equal(x.Size, tree.GetBoardSize());
                Equal(9, x.Size);
            });
        }

        [Fact]
        public void CaptureInASemeai()
        {
            var tree = SgfReader.Parse("(;AW[cn][bo][ap][bp][aq][cq][dq][ar][dr][er][as][ds]AB[cp][dp][ep][bq][eq][fq][gq][br][cr][fr][bs][es][fs])").Value.Single();

            var blackCaptures = tree.SetTurn(SgfColor.Black).PlayMove(SgfColor.Black, new Move.Point(3, 19), 19).GetBoard(19);
            Equal(5, blackCaptures.Captures[SgfColor.Black]);

            var whiteCaptures = tree.SetTurn(SgfColor.White).PlayMove(SgfColor.White, new Move.Point(3, 19), 19).GetBoard(19);
            Equal(4, whiteCaptures.Captures[SgfColor.White]);
        }

        [Fact]
        public void AddComment_NoPreviousComment_Comment()
        {
            var comment = new SgfText("apa");
            var tree = SgfGameTree.Empty.AddComment(comment);
            ContainsSingleProperty<C>(tree, x => Equal(comment, x.Comment));
        }

        [Fact]
        public void AddComment_PreviousComment_CommentAppendedToExistingComment()
        {
            var existingComment = new SgfText("apa");
            var newComment = new SgfText("bepa");

            var tree = SgfGameTree.Empty.AddComment(existingComment).AddComment(newComment);

            ContainsSingleProperty<C>(tree, x =>
            {
                var lines = x.Comment.Text.Split(Environment.NewLine);
                Equal(2, lines.Length);
                Equal("apa", lines[0]);
                Equal("bepa", lines[1]);
            });
        }

        [Fact]
        public void AddComment_AddedToLastNode()
        {
            var move = new Move.Point(3, 3);
            var comment = new SgfText("wow");
            var tree = SgfGameTree.Empty.PlayMove(SgfColor.Black, move, 19).AddComment(comment);
            SingleNode(tree, x =>
            {
                Equal(2, x.Properties.Count);
                var moveProperty = x.FindProperty<B>();
                NotNull(moveProperty);
                Equal(move, moveProperty.Move);

                var commentProperty = x.FindProperty<C>();
                Equal(comment, commentProperty.Comment);
            });
        }
    }
}
