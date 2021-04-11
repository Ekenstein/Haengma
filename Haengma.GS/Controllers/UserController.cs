using Haengma.GS.Actions;
using Haengma.GS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Haengma.GS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public Actions.ActionContext Actions { get; init; }

        public UserController(Actions.ActionContext actions)
        {
            Actions = actions;
        }

        [HttpGet]
        public ActionResult<IEnumerable<JsonUser>> GetUsers() => Actions.GetUsers(this);
    }
}
