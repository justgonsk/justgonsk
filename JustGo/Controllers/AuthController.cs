using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustGoModels.Models.Auth;
using JustGoUtilities.Exceptions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JustGo.Controllers
{
    [Route("api/[controller]/[action]")]
    [StubExceptionFilter]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly UserManager<JustGoUser> userManager;
        private readonly SignInManager<JustGoUser> signInManager;

        public AuthController(UserManager<JustGoUser> userManager, SignInManager<JustGoUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterData data)
        {
            if (ModelState.IsValid)
            {
                var user = new JustGoUser { UserName = data.Name, Email = data.Email };

                var result = await userManager.CreateAsync(user, data.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: true);
                    return Ok();
                }

                return Forbid();
            }

            return BadRequest(ModelState);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]  LoginData data)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager
                    .PasswordSignInAsync(data.Name, data.Password, isPersistent: true, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return Ok();
                }

                return Unauthorized();
            }

            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Info()
        {
            Console.WriteLine(HttpContext.Request.Cookies[".AspNetCore.Identity.Application"]);
            var currentUser = await userManager.GetUserAsync(HttpContext.User);

            return Ok(currentUser.ToViewModel());
        }

        public async Task AccessDenied()
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            await Response.WriteAsync("Invalid credentials");
        }
    }
}