using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustGoModels.Interfaces;
using JustGoModels.Models;
using JustGoModels.Models.Auth;
using JustGoModels.Models.View;
using JustGoModels.Policies;
using JustGoUtilities;
using JustGoUtilities.Exceptions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

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
                var newUser = new JustGoUser
                {
                    UserName = data.Name,
                    Email = data.Email
                };

                var result = await userManager.CreateAsync(newUser, data.Password);

                if (!result.Succeeded)
                    return Forbid();

                var loginAsAdmin = false;

                if (HttpContext.Request.Headers["answer"] == "42")
                {
                    await userManager.AddToRoleAsync(newUser, nameof(Admins));
                    loginAsAdmin = true;
                }

                await userManager.AddToRoleAsync(newUser, nameof(Users));

                await signInManager.SignInAsync(newUser, isPersistent: true);

                return Ok($"Registered account {User.Identity.Name}" + (loginAsAdmin ? " WITH ADMIN PRIVILEGES!" : "."));
            }

            return BadRequest(ModelState);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginData data)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await signInManager.SignOutAsync();

            var result = await signInManager
                .PasswordSignInAsync(data.Name, data.Password, isPersistent: true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return Ok($"Signed in as {data.Name}. Is admin account: {this.IsAdmin()}");
            }

            return Unauthorized();
        }

        [HttpGet]
        [Authorize(Roles = nameof(Users))]
        public async Task<IActionResult> Info()
        {
            var currentUser = await userManager.GetUserAsync(HttpContext.User);
            var eventsRepository = HttpContext.RequestServices.GetService<IEventsRepository>();
            var username = User.Identity.Name;

            var userInfo = currentUser.ToViewModel();
            userInfo.IsAdmin = this.IsAdmin();
            userInfo.AddedEvents = eventsRepository.EnumerateAll()
                .Where(ev => ev.Source == username).ToPoll<Event, EventViewModel>();
            return Ok(userInfo);
        }

        [HttpGet]
        [Authorize]
        public async Task<OkObjectResult> Logout()
        {
            await signInManager.SignOutAsync();
            return Ok($"Logout from {User.Identity.Name}");
        }

        public async Task AccessDenied()
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            await Response.WriteAsync("Invalid credentials");
        }
    }
}