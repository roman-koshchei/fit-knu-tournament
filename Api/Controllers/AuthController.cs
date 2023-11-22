using Data.Tables;
using Lib;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> userManager;
    private readonly Jwt jwt;

    public AuthController(UserManager<User> userManager, Jwt jwt)
    {
        this.userManager = userManager;
        this.jwt = jwt;
    }

    [HttpGet("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await userManager.FindByNameAsync(model.UserName);

        if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
        {
            return Unauthorized("Invalid credentials");
        }

        var token = jwt.Token(user.Id, user.Version);

        return Ok(new { Token = token, UserName = user.UserName, Email = user.Email });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var existingUser = await userManager.FindByNameAsync(model.UserName);
        if (existingUser != null)
        {
            return BadRequest("Username already exists");
        }

        var newUser = new User(model.Email);

        var result = await userManager.CreateAsync(newUser, model.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);

        var token = jwt.Token(newUser.Id, newUser.Version);

        return Ok(new { Token = token, UserName = newUser.UserName, Email = newUser.Email });
    }

    // Models for input data
    public record LoginModel(string UserName, string Password);

    public record RegisterModel(string UserName, string Email, string Password);
}