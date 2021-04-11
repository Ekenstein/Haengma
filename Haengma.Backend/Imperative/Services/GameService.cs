using Haengma.Backend.Functional.Sgf;
using Haengma.Backend.Imperative.Models;
using Haengma.Backend.Imperative.Persistance.Repositories;
using Haengma.Backend.Imperative.Services.Models;
using Haengma.Backend.Utils;
using System;
using System.Linq;
using static Haengma.Backend.Functional.Sgf.SgfProperty;

namespace Haengma.Backend.Imperative.Services
{
    public static class GameService
    {
        public static GameId NewGame(this ServiceContext serviceContext, GameOptions options) => serviceContext
            .Transactions
            .Write(t =>
            {
                var tree = SgfGameTree
                    .Empty
                    .AddRootProperty(new SZ(options.BoardSize))
                    .AddRootProperty(new KM(options.Komi));

                if (options.Handicap >= 2)
                {
                    tree = tree.PlaceFixedHandicap(options.Handicap, options.BoardSize);
                }

                var blackUser = t.GetUserById(options.Black);
                var whiteUser = t.GetUserById(options.White);

                tree = tree
                    .AddRootProperty(new PB(blackUser.Name))
                    .AddRootProperty(new PW(whiteUser.Name));

                var sgf = tree.ToSgf();
                var game = new Game(new GameId(Guid.NewGuid()), options.Black, options.White, sgf);
                t.InsertGame(game);
                return game.Id;
            });

        public static void PlayMove(
            this ServiceContext serviceContext,
            GameId gameId,
            Color color,
            Move move
        ) => serviceContext.Transactions.Write(t => {
            var game = t.GetGameById(gameId);
            var tree = GetGameTree(game.Sgf);

            var newTree = tree.Tree.PlayMove(color, move, tree.BoardSize);
            var sgf = SgfWriter.ToSgf(newTree);
            t.UpdateSgfForGame(gameId, sgf);
        });
        
        private static GameTreeWithInfo GetGameTree(string sgf)
        {
            var collection = SgfReader.Parse(sgf).GetValueOrDefault() ?? throw Throwable.Bug($"Couldn't parse the SGF '{sgf}'.");
            var tree = collection.SingleOrDefault() ?? throw Throwable.Bug($"The SGF {sgf} doesn't contain a tree.");
            
            var boardSize = tree.GetBoardSize() ?? 19;
            var komi = tree.GetKomi() ?? 0;
            var handicap = tree.GetHandicap() ?? 0;

            return new(tree, boardSize, komi, handicap);
        }

        public static GameInfo GetGame(this ServiceContext serviceContext, GameId gameId) => serviceContext
            .Transactions
            .Read(t => {
                var game = t.GetGameById(gameId);
                var tree = GetGameTree(game.Sgf);

                return new GameInfo(tree.BoardSize, tree.Komi, tree.Handicap, game.Black, game.White, game.Sgf);
            });

        public static string PrintGame(this ServiceContext serviceContext, GameId gameId) => serviceContext
            .Transactions
            .Read(t =>
            {
                var game = t.GetGameById(gameId);
                var tree = GetGameTree(game.Sgf);

                var board = tree.Tree.GetBoard(tree.BoardSize);
                return board.Print();
            });

        private record GameTreeWithInfo(SgfGameTree Tree, int BoardSize, double Komi, int Handicap);
    }
}
