using Data.Tables;
using Lib;
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

    [HttpGet]
    public IActionResult ExternalLogin()
    {
        var properties = signInManager.ConfigureExternalAuthenticationProperties("google", Url.Action("GoogleLoginCallback"));
        return Challenge(properties, "Google");
    }

    [HttpPost("callback")]
    public async Task<IActionResult> GoogleLoginCallback(string? remoteError = null)
    {
        if (remoteError != null)
        {
            return BadRequest($"Error from external provider: {remoteError}");
        }

        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return BadRequest("Error loading external login information.");
        }

        // Check if a user with the Google-registered email exists
        var user = await userManager.FindByEmailAsync(info.Principal.FindFirstValue(ClaimTypes.Email));

        if (user != null)
        {
            // User exists, sign them in
            await signInManager.SignInAsync(user, isPersistent: false);
            var token = jwt.Token(user.Id, user.Version);

            // Return the response as JSON
            return Ok(new { Token = token, UserName = user.UserName, Email = user.Email });
        }
        else
        {
            // User does not exist, create a new user
            var newUser = new User(email: info.Principal.FindFirstValue(ClaimTypes.Email));

            var result = await userManager.CreateAsync(newUser);
            if (!result.Succeeded)
            {
                return BadRequest("Error creating a new user.");
            }

            // Add the external login for the user
            result = await userManager.AddLoginAsync(newUser, info);
            if (!result.Succeeded)
            {
                return BadRequest("Error adding external login to the new user.");
            }

            // Sign in the newly created user
            await signInManager.SignInAsync(newUser, isPersistent: false);
            var token = jwt.Token(newUser.Id, newUser.Version);

            // Return the response as JSON
            return Ok(new { Token = token, UserName = newUser.UserName, Email = newUser.Email });
        }
    }
}