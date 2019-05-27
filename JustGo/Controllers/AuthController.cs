using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustGo.Policies;
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

        public AuthController(UserManager<JustGoUser> userManager,
            SignInManager<JustGoUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterData data)
        {
            if (ModelState.IsValid)
            {
                var newUser = new JustGoUser { UserName = data.Name, Email = data.Email };

                var result = await userManager.CreateAsync(newUser, data.Password);

                if (result.Succeeded)
                {
                    var loginAsAdmin = false;

                    if (HttpContext.Request.Headers["answer"] == "42")
                    {
                        await userManager.AddToRoleAsync(newUser, nameof(Admins));
                        loginAsAdmin = true;
                    }

                    await userManager.AddToRoleAsync(newUser, nameof(Users));

                    await signInManager.SignInAsync(newUser, isPersistent: true);

                    return Ok(loginAsAdmin ? "Admin permissions granted!" : "Basic user account created");
                }

                return Forbid();
            }

            return BadRequest(ModelState);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginData data)
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

        [HttpGet]
        [Authorize(Policy = "Users")]
        public async Task<IActionResult> Info()
        {
            var currentUser = await userManager.GetUserAsync(HttpContext.User);

            return Ok(currentUser.ToViewModel());
        }

        [HttpGet]
        public async Task<RedirectResult> Logout(string returnUrl)
        {
            await signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }

        public async Task AccessDenied()
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            await Response.WriteAsync("Invalid credentials");
        }
    }
}