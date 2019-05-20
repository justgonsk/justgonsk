using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustGoModels.Models.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JustGo.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly UserManager<JustGoUser> userManager;

        public UsersController(UserManager<JustGoUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserInfo()
        {
            var currentUser = await userManager.GetUserAsync(HttpContext.User);

            return Ok(currentUser);
        }
    }
}
