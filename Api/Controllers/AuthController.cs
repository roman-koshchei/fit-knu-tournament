using Data.Tables;
using Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly Jwt jwt;

    public AuthController(UserManager<User> userManager, Jwt jwt)
    {
        _userManager = userManager;
        this.jwt = jwt;
    }

    // GET api/login ROSTIK
    // - Logging in
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName);

        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
        {
            return Unauthorized("Invalid credentials");
        }

        var token = jwt.Token(user.Id, user.Version);

        return Ok(new { Token = token });
    }

    // POST api/register BOHDAN
    // - Registration
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var existingUser = await _userManager.FindByNameAsync(model.UserName);
        if (existingUser != null)
        {
            return BadRequest("Username already exists");
        }

        var newUser = new User { UserName = model.UserName, Email = model.Email };
        var result = await _userManager.CreateAsync(newUser, model.Password);

        if (result.Succeeded)
        {
            var token = jwt.Token(newUser.Id, newUser.Version);

            return Ok(new { Token = token });
        }

        return BadRequest(result.Errors);
    }

    // DELETE api/deleteUser ROMA KORINEVSKII
    [HttpDelete("deleteUser")]
    [Authorize]
    public async Task<IActionResult> Delete()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return NotFound("User not found");
        }

        if (user.Id != userId)
        {
            return Forbid();
        }

        var result = await _userManager.DeleteAsync(user);

        if (result.Succeeded)
        {
            return Ok("User deleted successfully");
        }

        return BadRequest(result.Errors);
    }
}

// Models for input data
public class LoginModel
{
    public string UserName { get; set; }
    public string Password { get; set; }
}

public class RegisterModel
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}