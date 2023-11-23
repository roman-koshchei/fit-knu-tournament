using Data.Tables;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.Config;
using Web.Services;
using System.Threading.Tasks;

namespace Web.Controllers;

/// <summary>
/// Controller for handling Google authentication-related actions.
/// </summary>
public class GoogleController : Controller
{
    private readonly SignInManager<User> signInManager;
    private readonly GoogleService googleService;
    private readonly UserManager<User> userManager;

    public GoogleController(SignInManager<User> signInManager, GoogleService googleService, UserManager<User> userManager)
    {
        this.signInManager = signInManager;
        this.googleService = googleService;
        this.userManager = userManager;
    }

    /// <summary>
    /// Initiates the Google authentication process.
    /// </summary>
    public IActionResult Index()
    {
        var redirect = "/Google/Callback";
        var properties = signInManager.ConfigureExternalAuthenticationProperties("google", redirect);
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Handles the callback from Google after authentication.
    /// </summary>
    /// <param name="remoteError">Error message from Google (if any).</param>
    public async Task<IActionResult> Callback(string? remoteError = null)
    {
        // Handle Google error, if any.
        if (remoteError != null) return Redirect("/"); // BadRequest($"Google error: {remoteError}");

        // Retrieve external login information.
        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info == null) return Redirect("/"); // BadRequest("Error loading external login information.");

        // Extract email from the external login information.
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (email == null) return Redirect("/"); // BadRequest("No email is found");

        // Check if the user already exists.
        var user = await userManager.FindByEmailAsync(email);
        if (user != null)
        {
            // Sign in the existing user with external authentication and generate a new token.
            var newToken = await googleService.SignInUserWithExternal(user, info);
            if (newToken == null) return Redirect("/");

            // Add the new authentication token to the response.
            Response.AddAuthCookie(newToken);
            return Redirect("/Account");
        }

        // Create a new user with external authentication and generate a token.
        var token = await googleService.CreateUserWithExternal(email, info);
        if (token == null) return Redirect("/"); // Problem("Error creating a new user.");

        // Add the new authentication token to the response.
        Response.AddAuthCookie(token);
        return Redirect("/Account");
    }
}
