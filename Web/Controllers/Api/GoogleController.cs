using Data.Tables;
using Lib;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Services;

namespace Web.Controllers.Api;

/// <summary>
/// API controller for Google authentication-related operations.
/// </summary>
[Route("api/auth/google")]
[ApiController]
public class GoogleController : ControllerBase
{
    private readonly SignInManager<User> signInManager;
    private readonly UserManager<User> userManager;
    private readonly Jwt jwt;
    private readonly GoogleService googleService;

    public GoogleController(SignInManager<User> signInManager, UserManager<User> userManager, Jwt jwt, GoogleService googleService)
    {
        this.signInManager = signInManager;
        this.userManager = userManager;
        this.jwt = jwt;
        this.googleService = googleService;
    }

    /// <summary>
    /// Represents the response output for the Login endpoint.
    /// </summary>
    public record UserInfo(string Token, string Email);

    /// <summary>
    /// Initiates the Google login process.
    /// </summary>
    [HttpGet]
    public IActionResult Login()
    {
        var redirect = "/api/auth/google/callback";
        var properties = signInManager.ConfigureExternalAuthenticationProperties("google", redirect);
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Handles the callback from Google after authentication.
    /// </summary>
    /// <param name="remoteError">Error message from Google (if any).</param>
    [HttpGet("callback")]
    public async Task<IActionResult> Callback(string? remoteError = null)
    {
        // Handle Google error, if any.
        if (remoteError != null) return BadRequest($"Google error: {remoteError}");

        // Retrieve external login information.
        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info == null) return BadRequest("Error loading external login information.");

        // Extract email from the external login information.
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (email == null) return BadRequest("No email is found");

        // Check if the user already exists.
        var user = await userManager.FindByEmailAsync(email);
        if (user != null)
        {
            // Sign in the existing user with external authentication and generate a new token.
            var newToken = await googleService.SignInUserWithExternal(user, info);
            if (newToken == null) return Problem();

            return Ok(new UserInfo(newToken, email));
        }

        // Create a new user with external authentication and generate a token.
        var token = await googleService.CreateUserWithExternal(email, info);
        if (token == null) return Problem("Error creating a new user.");

        return Ok(new UserInfo(token, email));
    }
}
