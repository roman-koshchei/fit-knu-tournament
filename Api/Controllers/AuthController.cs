using Data.Tables;
using Lib;
using Microsoft.AspNetCore.Authorization;
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

    public record LoginBody(string Email, string Password);

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginBody body)
    {
        var user = await userManager.FindByEmailAsync(body.Email);
        if (user == null) return NotFound();

        var passwordCorrect = await userManager.CheckPasswordAsync(user, body.Password);
        if (!passwordCorrect) return BadRequest("Invalid password");

        var token = jwt.Token(user.Id, user.Version);
        return Ok(new { Token = token });
    }

    public record RegisterBody(string Email, string Password, string ConfirmPassword);

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterBody body)
    {
        if (body.Password != body.ConfirmPassword) return BadRequest("Passwords don't match");

        {
            var existingUser = await userManager.FindByEmailAsync(body.Email);
            if (existingUser != null) return BadRequest("Email is already taken");
        }

        var user = new User(body.Email);

        var result = await userManager.CreateAsync(user, body.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);

        var token = jwt.Token(user.Id, user.Version);
        return Ok(new { Token = token });
    }

    [HttpGet("check")]
    [Authorize]
    public async Task<IActionResult> Check()
    {
        return Ok();
    }
}