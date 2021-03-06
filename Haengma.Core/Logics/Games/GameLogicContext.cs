﻿using Haengma.Core.Models;
using Haengma.Core.Persistence;
using Haengma.Core.Sgf;
using Haengma.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Haengma.Core.Utils.Collections;

namespace Haengma.Core.Logics.Games
{
    public record GameLogicContext(
        IGameNotifier Notifier,
        IDictionary<GameId, GameState> Games
    )
    {
        public async Task CreateGameAsync(ITransaction transaction,
            GameId gameId,
            GameSettings gameSettings,
            UserId blackPlayer,
            UserId whitePlayer)
        {
            var tree = CreateTreeWithGameSettings(gameSettings);
            var sgf = SgfWriter.ToSgf(tree);
            var game = new Game(gameId, blackPlayer, whitePlayer, sgf, gameSettings);

            transaction.AddGame(game);

            Games[gameId] = new GameState(
                MapOf(
                    blackPlayer.To(Color.Black),
                    whitePlayer.To(Color.White)
                ),
                new SemaphoreSlim(1, 1)
            );

            await Notifier.GameHasStartedAsync(gameId, blackPlayer, whitePlayer, GetBoardFromTree(gameSettings, tree));
        }

        private static Board GetBoardFromTree(GameSettings gameSettings, SgfGameTree tree) => tree
            .GetBoard(gameSettings.BoardSize)
            .Let(x => new Board(
                 gameSettings,
                 x.Captures[SgfColor.White],
                 x.Captures[SgfColor.Black],
                 x.Stones.Select(x => x.ToServiceModel()),
                 tree.NextTurn().ToServiceModel()
             ));

        private static async Task<(SgfGameTree tree, GameSettings settings)> GetSgfTreeAsync(IReadOnlyTransaction transaction, GameId gameId)
        {
            var game = await transaction.GetGameByIdAsync(gameId);
            var treeCollection = SgfReader.Parse(game.Sgf).GetValueOrDefault()
                ?? throw new InvalidOperationException($"Couldn't parse the SGF for the game {gameId}.");

            return (treeCollection[0], game.GameSettings);
        }

        public async Task AddMoveAsync(ITransaction transaction,
            GameId gameId,
            UserId userId,
            Models.Point point)
        {
            var game = GetGameState(gameId);
            await game.Lock.WaitAsync();
            try
            {
                var color = game.GetPlayerColor(userId);
                var (tree, settings) = await GetSgfTreeAsync(transaction, gameId);

                var move = new Move.Point(point.X, point.Y);

                var newTree = tree.PlayMove(
                    color.ToSgfModel(),
                    move,
                    settings.BoardSize
                );

                await UpdateSgfAsync(transaction, newTree, gameId);
                await Notifier.MoveAddedAsync(gameId, GetBoardFromTree(settings, newTree));
            }
            finally
            {
                game.Lock.Release();
            }
        }

        public async Task PassAsync(ITransaction transaction,
            GameId gameId,
            UserId userId)
        {
            var game = GetGameState(gameId);
            await game.Lock.WaitAsync();
            try
            {
                var color = game.GetPlayerColor(userId);
                var (tree, settings) = await GetSgfTreeAsync(transaction, gameId);

                var move = new Move.Pass();
                var newTree = tree.PlayMove(color.ToSgfModel(), move, settings.BoardSize);

                await UpdateSgfAsync(transaction, newTree, gameId);
                await Notifier.PlayerPassedAsync(gameId, color);

                if (newTree.HasGameEnded())
                {
                    await Notifier.GameHasEndedAsync(gameId);
                }
            }
            finally
            {
                game.Lock.Release();
            }
        }

        public async Task AddCommentAsync(ITransaction transaction,
            GameId gameId,
            string comment)
        {
            var (tree, _) = await GetSgfTreeAsync(transaction, gameId);

            await UpdateSgfAsync(transaction, tree.AddComment(SgfText.FromString(comment)), gameId);
            await Notifier.CommentAddedAsync(gameId, comment);
        }

        public async Task SendEmote(ITransaction transaction, GameId gameId, UserId userId, Emote emote)
        {
            var gameState = GetGameState(gameId);
            var color = gameState.GetPlayerColor(userId);
            var (tree, _) = await GetSgfTreeAsync(transaction, gameId);

            var newTree = tree.AddEmote(color.ToSgfModel(), emote.ToSgfModel());

            await UpdateSgfAsync(transaction, newTree, gameId);
            await Notifier.EmoteSentAsync(gameId, emote, color);
        }

        public async Task ResignAsync(ITransaction transaction,
            GameId gameId,
            UserId userId)
        {
            var game = GetGameState(gameId);
            var color = game.GetPlayerColor(userId);
            var (tree, _) = await GetSgfTreeAsync(transaction, gameId);

            var newTree = tree.Resign(color.ToSgfModel());

            await UpdateSgfAsync(transaction, newTree, gameId);
            await Notifier.PlayerResignedAsync(gameId, color);
            await Notifier.GameHasEndedAsync(gameId);
        }

        private GameState GetGameState(GameId gameId) => Games[gameId]
            ?? throw new ArgumentException($"There's no active game with the id {gameId}.");

        private static async Task UpdateSgfAsync(ITransaction transaction, SgfGameTree gameTree, GameId gameId)
        {
            var sgf = SgfWriter.ToSgf(gameTree);
            await transaction.UpdateSgfAsync(gameId, sgf);
        }

        private static SgfGameTree CreateTreeWithGameSettings(GameSettings gameSettings)
        {
            var tree = SgfGameTree
                .Empty
                .SetBoardSize(gameSettings.BoardSize)
                .SetKomi(gameSettings.Komi);

            if (gameSettings.Handicap >= 2)
            {
                tree = tree.PlaceFixedHandicap(gameSettings.Handicap, gameSettings.BoardSize);
            }

            return tree;
        }
    }
}
