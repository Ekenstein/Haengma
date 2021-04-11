using Haengma.GS.Actions;
using Haengma.GS.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Haengma.GS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        public Actions.ActionContext Actions { get; }

        public GameController(Actions.ActionContext actionContext)
        {
            Actions = actionContext;
        }

        [HttpPost]
        public ActionResult<Guid> NewGame(JsonPostGameOptions options) => Actions.PostNewGame(options);

        [HttpPost]
        [Route("{gameId}")]
        public IActionResult PlayMove(Guid gameId, JsonPostMove move) => Actions.PostPlayMove(gameId, move);

        [HttpGet]
        [Route("{gameId}")]
        public ActionResult<JsonGame> GetGame(Guid gameId) => Actions.GetGame(gameId);

        [HttpGet]
        [Route("{gameId}/print")]
        public ActionResult<string> PrintGame(Guid gameId) => Actions.GetPrintGame(gameId);
    }
}
