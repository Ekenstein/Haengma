using Haengma.Backend.Functional.Sgf;
using Haengma.Backend.Imperative.Models;
using Haengma.Backend.Imperative.Services;
using Haengma.Backend.Utils;
using Haengma.GS.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Haengma.GS.Actions
{
    public static class GameActions
    {
        public static ActionResult<Guid> PostNewGame(this ActionContext actionContext, 
            JsonPostGameOptions options)
        {
            var id = actionContext.Services.NewGame(options.ToModel());
            return new OkObjectResult(id.Id);
        }

        public static IActionResult PostPlayMove(this ActionContext actionContext, 
            Guid id, 
            JsonPostMove move)
        {
            var color = move.Color.ToModel();
            var gameId = new GameId(id);
            actionContext.Services.PlayMove(gameId,
                color,
                move.ToMove());

            return new OkResult();
        }

        public static ActionResult<JsonGame> GetGame(this ActionContext actionContext, Guid gameId) => actionContext
            .Services
            .GetGame(new GameId(gameId))
            .Map(x => new OkObjectResult(x.ToJson()));

        public static ActionResult<string> GetPrintGame(this ActionContext actionContext, Guid gameId) => actionContext
            .Services
            .PrintGame(new GameId(gameId))
            .Map(x => new OkObjectResult(x));
    }
}
