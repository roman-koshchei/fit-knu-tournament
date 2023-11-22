using Data.Tables;
using Lib;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

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

    [HttpPost("login")]
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

    [HttpPost("google-login")]
    public IActionResult ExternalLogin()
    {
        var properties = _signInManager.ConfigureExternalAuthenticationProperties("google", Url.Action("GoogleLoginCallback"));
        return Challenge(properties, "Google");
    }

    [HttpGet("google-login-callback")]
    public async Task<IActionResult> GoogleLoginCallback(string? remoteError = null)
    {
        if (remoteError != null)
        {
            return BadRequest($"Error from external provider: {remoteError}");
        }

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return BadRequest("Error loading external login information.");
        }

        // Check if a user with the Google-registered email exists
        var user = await _userManager.FindByEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email));

        if (user != null)
        {
            // User exists, sign them in
            await _signInManager.SignInAsync(user, isPersistent: false);
            var token = _jwt.Token(user.Id, user.Version);

            // Return the response as JSON
            return Ok(new { Token = token, UserName = user.UserName, Email = user.Email });
        }
        else
        {
            // User does not exist, create a new user
            var newUser = new User(email: info.Principal.FindFirstValue(ClaimTypes.Email));

            var result = await _userManager.CreateAsync(newUser);
            if (!result.Succeeded)
            {
                return BadRequest("Error creating a new user.");
            }

            // Add the external login for the user
            result = await _userManager.AddLoginAsync(newUser, info);
            if (!result.Succeeded)
            {
                return BadRequest("Error adding external login to the new user.");
            }

            // Sign in the newly created user
            await _signInManager.SignInAsync(newUser, isPersistent: false);
            var token = _jwt.Token(newUser.Id, newUser.Version);

            // Return the response as JSON
            return Ok(new { Token = token, UserName = newUser.UserName, Email = newUser.Email });
        }
    }

    // Models for input data
    public record LoginModel(string UserName, string Password);

    public record RegisterModel(string UserName, string Email, string Password);

    public class ExternalLoginConfirmationModel
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        // Add any additional properties needed for registration or confirmation.
    }
}