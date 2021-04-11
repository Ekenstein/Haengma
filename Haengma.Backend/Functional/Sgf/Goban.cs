using Haengma.Backend.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Haengma.Backend.Functional.Sgf.SgfProperty;

namespace Haengma.Backend.Functional.Sgf
{
    public record Board(Set<Stone> Stones, Map<Color, int> Captures, int BoardSize) { 
        public static Board Empty(int boardSize) => new(Set.Empty<Stone>(), Map.Of((Color.Black, 0), (Color.White, 0)), boardSize);

        public bool IsOccupied(Point point) => Stones.Any(x => x.Point == point);
    }

    public static class BoardExtensions
    {
        public static string Print(this Board board)
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
                        .Cast<Color?>()
                        .SingleOrDefault();

                    switch (stone)
                    {
                        case null: sb.Append('.'); break;
                        case Color.Black: sb.Append('#'); break;
                        case Color.White: sb.Append('O'); break;
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
        public static SgfGameTree SetBoardSize(this SgfGameTree tree, int boardSize) => tree.AddRootProperty(new SZ(boardSize));
        public static int? GetHandicap(this SgfGameTree tree) => tree.RootNode()?.FindProperty<HA>()?.Handicap;

        public static bool IsBoardEmpty(this SgfGameTree tree) => !tree.GetStones().Any();

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

        public static SgfGameTree PlayMove(this SgfGameTree tree, Color color, Move move, int boardSize)
        {
            return move switch
            {
                Move.Pass => tree.Pass(color),
                Move.Point p => tree.PlaceStone(new Stone(color, new Point(p.X, p.Y)), boardSize),
                _ => throw new ArgumentOutOfRangeException(nameof(move), move, "Couldn't recognize the given move.")
            };
        }

        private static SgfGameTree Pass(this SgfGameTree tree, Color color) => color switch
        {
            Color.Black => tree.AppendNode(new B(new Move.Pass()).AsNode()),
            Color.White => tree.AppendNode(new W(new Move.Pass()).AsNode()),
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
                Color.Black => tree.AppendNode(new B(new Move.Point(stone.Point.X, stone.Point.Y)).AsNode()),
                Color.White => tree.AppendNode(new W(new Move.Point(stone.Point.X, stone.Point.Y)).AsNode()),
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
            AB ab => ab.Stones.Select(x => new Stone(Color.Black, x)).ToArray(),
            AW aw => aw.Stones.Select(x => new Stone(Color.White, x)).ToArray(),
            W w => w.Move.ToPoint()?.Map(x => List.Of(new Stone(Color.White, x))) ?? List.Empty<Stone>(),
            B b => b.Move.ToPoint()?.Map(x => List.Of(new Stone(Color.Black, x))) ?? List.Empty<Stone>(),
            _ => List.Empty<Stone>()
        };

        private static IReadOnlyList<Stone> GetStones(this SgfGameTree tree) => tree
            .Sequence
            .SelectMany(x => x.Properties)
            .SelectMany(x => x.GetStones())
            .ToArray();

        private static Set<Point> AdjacentPoints(this Point point, int boardSize) => Set.Of(
            new Point(point.X - 1, point.Y),
            new Point(point.X + 1, point.Y),
            new Point(point.X, point.Y - 1),
            new Point(point.X, point.Y + 1)
        ).Where(x => x.X >= 1 && x.X <= boardSize && x.Y >= 1 && x.Y <= boardSize).ToSet();

        private static (Set<Stone> group, int liberties) GetLiberties(this Board board, 
            Stone stone)
        {
            Map<Stone, int> Liberties(Map<Stone, int> currentGroup, Stone stone)
            {
                var adjacentPoints = stone.Point.AdjacentPoints(board.BoardSize);

                var adjacentStones = board.Stones.Where(x => adjacentPoints.Contains(x.Point));
                var liberties = adjacentPoints.Count - adjacentStones.Count();

                var newGroup = currentGroup + Map.Of((stone, liberties));
                var friendlyStones = adjacentStones
                    .Where(x => x.Color == stone.Color)
                    .Where(x => !newGroup.Keys.Contains(x));

                return friendlyStones
                    .Aggregate(newGroup, (group, stone) => group + Liberties(group, stone));
            }

            return Liberties(Map.Empty<Stone, int>(), stone)
                .Map(x => (x.Keys.ToSet(), x.Values.Sum()));
        }

        public static Board GetBoard(this SgfGameTree tree, int boardSize) => tree
            .GetStones()
            .Aggregate(Board.Empty(boardSize), (board, stone) => {
                var newBoard = board with {
                    Stones = board.Stones + Set.Of(stone)
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
                            Stones = board.Stones - group,
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

        private static Set<Point> GetHandicapPoints(int boardSize, int handicap)
        {
            var edgeDistance = GetEdgeDistance(boardSize);
            if (edgeDistance == null)
            {
                return Set.Empty<Point>();
            }

            return getHandicap(handicap, edgeDistance.Value, boardSize);

            static Set<Point> getHandicap(int handicap, int edgeDistance, int boardSize) => handicap switch {
                2 => Set.Of(
                    new Point(edgeDistance, boardSize - edgeDistance + 1),
                    new Point(boardSize - edgeDistance + 1, edgeDistance)
                ),
                3 => Set.Of(
                    new Point(boardSize - edgeDistance + 1, boardSize - edgeDistance + 1)
                ) + getHandicap(2, edgeDistance, boardSize),
                4 => Set.Of(
                    new Point(edgeDistance, edgeDistance)
                ) + getHandicap(3, edgeDistance, boardSize),
                5 => Set.Of(
                    Tengen(boardSize)
                ) + getHandicap(4, edgeDistance, boardSize),
                6 => Set.Of(
                    new Point(edgeDistance, Middle(boardSize)),
                    new Point(boardSize - edgeDistance + 1, Middle(boardSize))
                ) + getHandicap(4, edgeDistance, boardSize),
                7 => Set.Of(
                    Tengen(boardSize)
                ) + getHandicap(6, edgeDistance, boardSize),
                8 => Set.Of(
                    new Point(Middle(boardSize), edgeDistance),
                    new Point(Middle(boardSize), boardSize - edgeDistance + 1)
                ) + getHandicap(6, edgeDistance, boardSize),
                9 => Set.Of(Tengen(boardSize)) + getHandicap(8, edgeDistance, boardSize),
                _ => Set.Empty<Point>()
            };
        }
    }
}
