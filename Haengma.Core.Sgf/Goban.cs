using Haengma.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Haengma.Core.Sgf.SgfProperty;
using static Haengma.Core.Utils.Collections;

namespace Haengma.Core.Sgf
{
    public record GoBoard(IReadOnlySet<Stone> Stones, IReadOnlyDictionary<SgfColor, int> Captures, int BoardSize)
    {
        public static GoBoard Empty(int boardSize) => new(EmptySet<Stone>(), MapOf(SgfColor.Black.To(0), SgfColor.White.To(0)), boardSize);

        public bool IsOccupied(Point point) => Stones.Any(x => x.Point == point);
    }

    public static class BoardExtensions
    {
        public static string Print(this GoBoard board)
        {
            var sb = new StringBuilder();

            for (var y = 1; y <= board.BoardSize; y++)
            {
                for (var x = 1; x <= board.BoardSize; x++)
                {
                    var point = new Point(x, y);
                    var stone = board.Stones
                        .Where(x => x.Point == point)
                        .Select(x => x.Color)
                        .Cast<SgfColor?>()
                        .SingleOrDefault();

                    switch (stone)
                    {
                        case null: sb.Append('.'); break;
                        case SgfColor.Black: sb.Append('#'); break;
                        case SgfColor.White: sb.Append('O'); break;
                    }
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }

    public static class Goban
    {
        public static int? GetBoardSize(this SgfGameTree tree) => tree.RootNode()?.FindProperty<SZ>()?.Size;
        public static double? GetKomi(this SgfGameTree tree) => tree.RootNode()?.FindProperty<KM>()?.Komi;
        public static string? GetBlackPlayerName(this SgfGameTree tree) => tree.RootNode()?.FindProperty<PB>()?.Name;
        public static string? GetWhitePlayerName(this SgfGameTree tree) => tree.RootNode()?.FindProperty<PW>()?.Name;
        public static SgfGameTree SetBoardSize(this SgfGameTree tree, int boardSize) => tree.AddRootProperty(new SZ(boardSize));
        public static SgfGameTree SetKomi(this SgfGameTree tree, double komi) => tree.AddRootProperty(new KM(komi));
        public static SgfGameTree SetBlackPlayerName(this SgfGameTree tree, string name) => tree.AddRootProperty(new PB(name));
        public static SgfGameTree SetWhitePlayerName(this SgfGameTree tree, string name) => tree.AddRootProperty(new PW(name));
        public static SgfGameTree SetBlackPlayerRank(this SgfGameTree tree, string rank) => tree.AddRootProperty(new BR(rank));
        public static SgfGameTree SetWhitePlayerRank(this SgfGameTree tree, string rank) => tree.AddRootProperty(new WR(rank));
        public static int? GetHandicap(this SgfGameTree tree) => tree.RootNode()?.FindProperty<HA>()?.Handicap;
        public static bool HasGameEnded(this SgfGameTree tree) => tree.RootNode()?.FindProperty<RE>() != null || tree.HasTwoConsecutivePasses();
        public static SgfGameTree SetTurn(this SgfGameTree tree, SgfColor color) => tree.AppendNode(new PL(color).AsNode());
        public static SgfGameTree AddEmote(this SgfGameTree tree, SgfColor color, SgfEmote emote) => tree.AppendPropertyToLastNode(new Emote(color, emote));
        public static bool HasTurn(this SgfGameTree tree, SgfColor color) => !tree.HasGameEnded() && tree
            .Sequence
            .SelectMany(x => x.Properties)
            .SelectNotNull(x => x switch
            {
                PL pl => pl.Color,
                B b => SgfColor.White,
                W w => SgfColor.Black,
                AB => SgfColor.White,
                AW => SgfColor.Black,
                _ => default
            })
            .DefaultIfEmpty(SgfColor.Black)
            .LastOrDefault() == color;

        public static SgfColor NextTurn(this SgfGameTree tree) => tree.HasTurn(SgfColor.Black) ? SgfColor.Black : SgfColor.White;

        public static bool IsBoardEmpty(this SgfGameTree tree) => !tree.GetStones().Any();

        public static bool HasTwoConsecutivePasses(this SgfGameTree tree) => tree
            .Sequence
            .Reverse()
            .Take(2)
            .SelectMany(x => x.Properties)
            .Where(x => x.IsPass())
            .Count() == 2;

        public static SgfGameTree AddComment(this SgfGameTree tree, string comment)
        {
            C AddComment() => new(comment);
            C UpdateComment(C property) => property with
            {
                Comment = property.Comment + "\n" + comment
            };

            return tree.AddOrUpdatePropertyOnLastNode(
                UpdateComment,
                AddComment
            );
        }

        public static SgfGameTree PlaceFixedHandicap(this SgfGameTree tree, int handicap, int boardSize)
        {
            if (!tree.IsBoardEmpty())
            {
                throw new SgfException("The board isn't empty.");
            }

            var maxHandicap = GetMaxHandicap(boardSize);
            if (handicap < 2 || handicap > maxHandicap)
            {
                throw new SgfException($"Handicap must be larger than or equal to 2 and less than or equal to {maxHandicap}.");
            }

            return tree
                .AddRootProperty(new HA(handicap))
                .AddRootProperty(new AB(GetHandicapPoints(boardSize, handicap)));
        }

        public static SgfGameTree Resign(this SgfGameTree tree, SgfColor color)
        {
            if (tree.HasGameEnded())
            {
                throw new SgfException("The game has already ended.");
            }

            var result = color switch
            {
                SgfColor.Black => "W+R",
                _ => "B+R"
            };

            return tree.AddRootProperty(new RE(result));
        }

        public static SgfGameTree PlayMove(this SgfGameTree tree, SgfColor color, Move move, int boardSize)
        {
            if (tree.HasGameEnded())
            {
                throw new SgfException("The game has ended.");
            }

            if (!tree.HasTurn(color))
            {
                throw new SgfException($"It's not {color}'s turn.");
            }

            return move switch
            {
                Move.Pass => tree.Pass(color),
                Move.Point p => tree.PlaceStone(new Stone(color, new Point(p.X, p.Y)), boardSize),
                _ => throw new ArgumentOutOfRangeException(nameof(move), move, "Couldn't recognize the given move.")
            };
        }

        private static SgfGameTree Pass(this SgfGameTree tree, SgfColor color) => color switch
        {
            SgfColor.Black => tree.AppendNode(new B(new Move.Pass()).AsNode()),
            SgfColor.White => tree.AppendNode(new W(new Move.Pass()).AsNode()),
            _ => throw new ArgumentOutOfRangeException(nameof(color), color, "Couldn't recognize the given color.")
        };

        private static SgfGameTree PlaceStone(this SgfGameTree tree, Stone stone, int boardSize)
        {
            var board = tree.GetBoard(boardSize);
            if (board.IsOccupied(stone.Point))
            {
                throw new SgfException($"The point ({stone.Point.X};{stone.Point.Y}) is occupied.");
            }

            var newTree = stone.Color switch
            {
                SgfColor.Black => tree.AppendNode(new B(new Move.Point(stone.Point.X, stone.Point.Y)).AsNode()),
                SgfColor.White => tree.AppendNode(new W(new Move.Point(stone.Point.X, stone.Point.Y)).AsNode()),
                _ => throw new NotImplementedException()
            };

            var liberties = newTree.GetBoard(boardSize).GetLiberties(stone).liberties;
            if (liberties <= 0)
            {
                throw new SgfException($"Playing at the given point ({stone.Point.X};{stone.Point.Y}) is suicide.");
            }

            return newTree;
        }

        private static IReadOnlyList<Stone> GetStones(this SgfProperty property) => property switch
        {
            AB ab => ab.Stones.Select(x => new Stone(SgfColor.Black, x)).ToArray(),
            AW aw => aw.Stones.Select(x => new Stone(SgfColor.White, x)).ToArray(),
            W w => w.Move.ToPoint()?.Let(x => List.Of(new Stone(SgfColor.White, x))) ?? List.Empty<Stone>(),
            B b => b.Move.ToPoint()?.Let(x => List.Of(new Stone(SgfColor.Black, x))) ?? List.Empty<Stone>(),
            _ => List.Empty<Stone>()
        };

        private static IReadOnlyList<Stone> GetStones(this SgfGameTree tree) => tree
            .Sequence
            .SelectMany(x => x.Properties)
            .SelectMany(x => x.GetStones())
            .ToArray();

        private static IReadOnlySet<Point> AdjacentPoints(this Point point, int boardSize) => SetOf(
            new Point(point.X - 1, point.Y),
            new Point(point.X + 1, point.Y),
            new Point(point.X, point.Y - 1),
            new Point(point.X, point.Y + 1)
        ).Where(x => x.X >= 1 && x.X <= boardSize && x.Y >= 1 && x.Y <= boardSize).ToSet();

        private static (IReadOnlySet<Stone> group, int liberties) GetLiberties(this GoBoard board,
            Stone stone)
        {
            IReadOnlyDictionary<Stone, int> Liberties(IReadOnlyDictionary<Stone, int> currentGroup, Stone stone)
            {
                var adjacentPoints = stone.Point.AdjacentPoints(board.BoardSize);

                var adjacentStones = board.Stones.Where(x => adjacentPoints.Contains(x.Point));
                var liberties = adjacentPoints.Count - adjacentStones.Count();

                var newGroup = currentGroup.Merge(MapOf(stone.To(liberties)));
                var friendlyStones = adjacentStones
                    .Where(x => x.Color == stone.Color)
                    .Where(x => !newGroup.Keys.Contains(x));

                return friendlyStones
                    .Aggregate(newGroup, (group, stone) => group.Merge(Liberties(group, stone)));
            }

            return Liberties(EmptyMap<Stone, int>(), stone).Let(x => (x.Keys.ToSet(), x.Values.Sum()));
        }

        public static GoBoard GetBoard(this SgfGameTree tree, int boardSize) => tree
            .GetStones()
            .Aggregate(GoBoard.Empty(boardSize), (board, stone) =>
            {
                var newBoard = board with
                {
                    Stones = board.Stones.Merge(SetOf(stone))
                };

                var adjacentPoints = stone
                    .Point
                    .AdjacentPoints(boardSize);

                var adjacentStones = newBoard
                    .Stones
                    .Where(x => adjacentPoints.Contains(x.Point))
                    .ToSet();

                var enemyAdjacentStones = adjacentStones
                    .Where(x => x.Color != stone.Color)
                    .ToSet();

                return enemyAdjacentStones.Aggregate(newBoard, (board, stone) =>
                {
                    var (group, liberties) = board.GetLiberties(stone);
                    return liberties <= 0
                        ? board with
                        {
                            Stones = board.Stones.Subset(group),
                            Captures = board.Captures.MapKey(stone.Color.Inverse(), n => n + group.Count)
                        }
                        : board;
                });
            });

        private static int? GetEdgeDistance(int boardSize) => boardSize switch
        {
            var n when n < 13 => 3,
            var n when n < 7 => null,
            _ => 4
        };

        public static int GetMaxHandicap(int boardSize) => boardSize switch
        {
            var n when n < 7 => 0,
            var n when n == 7 => 4,
            var n when n % 2 == 0 => 4,
            _ => 9
        };

        private static Point Tengen(int boardSize) => new(Middle(boardSize), Middle(boardSize));
        private static int Middle(int boardSize) => (int)Math.Ceiling(boardSize / 2d);

        private static IReadOnlySet<Point> GetHandicapPoints(int boardSize, int handicap)
        {
            var edgeDistance = GetEdgeDistance(boardSize);
            if (edgeDistance == null)
            {
                return EmptySet<Point>();
            }

            return getHandicap(handicap, edgeDistance.Value, boardSize);

            static IReadOnlySet<Point> getHandicap(int handicap, int edgeDistance, int boardSize) => handicap switch
            {
                2 => SetOf(
                    new Point(edgeDistance, boardSize - edgeDistance + 1),
                    new Point(boardSize - edgeDistance + 1, edgeDistance)
                ),
                3 => SetOf(
                    new Point(boardSize - edgeDistance + 1, boardSize - edgeDistance + 1)
                ).Merge(getHandicap(2, edgeDistance, boardSize)),
                4 => SetOf(
                    new Point(edgeDistance, edgeDistance)
                ).Merge(getHandicap(3, edgeDistance, boardSize)),
                5 => SetOf(
                    Tengen(boardSize)
                ).Merge(getHandicap(4, edgeDistance, boardSize)),
                6 => SetOf(
                    new Point(edgeDistance, Middle(boardSize)),
                    new Point(boardSize - edgeDistance + 1, Middle(boardSize))
                ).Merge(getHandicap(4, edgeDistance, boardSize)),
                7 => SetOf(
                    Tengen(boardSize)
                ).Merge(getHandicap(6, edgeDistance, boardSize)),
                8 => SetOf(
                    new Point(Middle(boardSize), edgeDistance),
                    new Point(Middle(boardSize), boardSize - edgeDistance + 1)
                ).Merge(getHandicap(6, edgeDistance, boardSize)),
                9 => SetOf(Tengen(boardSize)).Merge(getHandicap(8, edgeDistance, boardSize)),
                _ => EmptySet<Point>()
            };
        }
    }
}
