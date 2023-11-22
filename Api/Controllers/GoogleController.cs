using Data.Tables;
using Lib;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[Route("api/auth/google")]
[ApiController]
public class GoogleController : ControllerBase
{
    private readonly SignInManager<User> signInManager;
    private readonly UserManager<User> userManager;
    private readonly Jwt jwt;

    public GoogleController(SignInManager<User> signInManager, UserManager<User> userManager, Jwt jwt)
    {
        this.signInManager = signInManager;
        this.userManager = userManager;
        this.jwt = jwt;
    }

    public record UserInfo(string Token, string Email);

    [HttpGet]
    public IActionResult Login()
    {
        var redirect = "/api/auth/google/callback";
        var properties = signInManager.ConfigureExternalAuthenticationProperties("google", redirect);
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback(string? remoteError = null)
    {
        if (remoteError != null) return BadRequest($"Google error: {remoteError}");

        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info == null) return BadRequest("Error loading external login information.");

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (email == null) return BadRequest("No email is found");

        var user = await userManager.FindByEmailAsync(email);
        if (user != null)
        {
            var newToken = await SignInUserWithExternal(user, info);
            if (newToken == null) return Problem();

            return Ok(new UserInfo(newToken, email));
        }

        var token = await CreateUserWithExternal(email, info);
        if (token == null) return Problem("Error creating a new user.");

        return Ok(new UserInfo(token, email));
    }

    [NonAction]
    private async Task<string?> SignInUserWithExternal(User user, ExternalLoginInfo loginInfo)
    {
        var signInResult = await signInManager.ExternalLoginSignInAsync(
            loginInfo.LoginProvider, loginInfo.ProviderKey, isPersistent: false, bypassTwoFactor: true
        );
        if (!signInResult.Succeeded) return null;

        return jwt.Token(user.Id, user.Version);
    }

    /// <summary>
    /// Create new user for external auth and new user token.
    /// </summary>
    /// <returns>Token if success, otherwise null</returns>
    [NonAction]
    private async Task<string?> CreateUserWithExternal(string email, ExternalLoginInfo loginInfo)
    {
        var newUser = new User(email);

        var result = await userManager.CreateAsync(newUser);
        if (!result.Succeeded) return null;

        // Add the external login for the user
        result = await userManager.AddLoginAsync(newUser, loginInfo);
        if (!result.Succeeded) return null;

        return jwt.Token(newUser.Id, newUser.Version);
    }
}