using Data.Tables;
using Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Api;

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

    public record LoginModel(string Email, string Password);

    [HttpGet("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null) return NotFound();

        var passwordCorrect = await userManager.CheckPasswordAsync(user, model.Password);
        if (!passwordCorrect) return BadRequest();

        var token = jwt.Token(user.Id, user.Version);
        return Ok(new { Token = token, user.Email });
    }

    public record RegisterModel(string Email, string Password, string ConfirmPassword);

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (model.Password != model.ConfirmPassword) return BadRequest("Passwords don't match");

        var existingUser = await userManager.FindByEmailAsync(model.Email);
        if (existingUser != null) return Conflict("Email is already taken");

        var newUser = new User(model.Email);

        var result = await userManager.CreateAsync(newUser, model.Password);
        if (!result.Succeeded) return StatusCode(500, result.Errors);

        var token = jwt.Token(newUser.Id, newUser.Version);
        return Ok(new { Token = token, newUser.Email });
    }

    [HttpGet("check")]
    [Authorize]
    public IActionResult Check()
    {
        return Ok();
    }
}