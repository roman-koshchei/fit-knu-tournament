using Data.Tables;
using Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using System.Xml;

namespace Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly Jwt _jwt;

        public AuthController(SignInManager<User> signInManager, UserManager<User> userManager, Jwt jwt)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwt = jwt;
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Unauthorized("Invalid credentials");
            }

            var token = _jwt.Token(user.Id, user.Version);

            return Ok(new { Token = token, UserName = user.UserName, Email = user.Email });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var existingUser = await _userManager.FindByNameAsync(model.UserName);
            if (existingUser != null)
            {
                return BadRequest("Username already exists");
            }

            var newUser = new User(model.Email);

            var result = await _userManager.CreateAsync(newUser, model.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            var token = _jwt.Token(newUser.Id, newUser.Version);

            return Ok(new { Token = token, UserName = newUser.UserName, Email = newUser.Email });
        }


        // Models for input data
        public record LoginModel(string UserName, string Password);

        public record RegisterModel(string UserName, string Email, string Password);

    }
}

