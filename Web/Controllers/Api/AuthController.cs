using Data.Tables;
using Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.Api;

/// <summary>
/// API controller for authentication-related operations.
/// </summary>
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

    /// <summary>
    /// Represents the request body for the login endpoint.
    /// </summary>
    public record LoginModel(string Email, string Password);

    /// <summary>
    /// Handles user login and generates an authentication token.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        // Attempt to find the user by email.
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null) return NotFound();

        // Check if the provided password is correct.
        var passwordCorrect = await userManager.CheckPasswordAsync(user, model.Password);
        if (!passwordCorrect) return BadRequest();

        // Generate a JWT token for the authenticated user.
        var token = jwt.Token(user.Id, user.Version);
        return Ok(new { Token = token, user.Email });
    }

    /// <summary>
    /// Represents the request body for the registration endpoint.
    /// </summary>
    public record RegisterModel(string Email, string Password, string ConfirmPassword);

    /// <summary>
    /// Handles user registration and generates an authentication token.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        // Check if passwords match.
        if (model.Password != model.ConfirmPassword) return BadRequest("Passwords don't match");

        // Check if an existing user has the same email.
        var existingUser = await userManager.FindByEmailAsync(model.Email);
        if (existingUser != null) return Conflict("Email is already taken");

        // Create a new user and attempt to persist it.
        var newUser = new User(model.Email);

        var result = await userManager.CreateAsync(newUser, model.Password);
        if (!result.Succeeded) return StatusCode(500, result.Errors);

        // Generate a JWT token for the newly registered user.
        var token = jwt.Token(newUser.Id, newUser.Version);
        return Ok(new { Token = token, newUser.Email });
    }

    /// <summary>
    /// Checks the validity of the authentication token.
    /// </summary>
    [HttpGet("check")]
    [Authorize]
    public IActionResult Check()
    {
        return Ok();
    }
}
