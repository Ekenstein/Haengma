using Haengma.Backend.Imperative.Services;
using Haengma.GS.Controllers;
using Haengma.GS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Haengma.GS.Actions
{
    public static class UserActions
    {
        public static ActionResult<IEnumerable<JsonUser>> GetUsers(this ActionContext actionContext, UserController controller)
        {
            var users = actionContext.Services.GetUsers().Select(x => x.ToJson());
            return controller.Ok(users);
        }
    }
}
