using Data.Tables;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Configuration;
using static Web.Controllers.Api.GoogleController;
using System.Security.Claims;
using Web.Services;

namespace Web.Controllers;

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

    public IActionResult Index()
    {
        var redirect = "/Google/Callback";
        var properties = signInManager.ConfigureExternalAuthenticationProperties("google", redirect);
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    public async Task<IActionResult> Callback(string? remoteError = null)
    {
        if (remoteError != null) return Redirect("/"); // BadRequest($"Google error: {remoteError}");

        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info == null) return Redirect("/"); //BadRequest("Error loading external login information.");

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (email == null) return Redirect("/"); //BadRequest("No email is found");

        var user = await userManager.FindByEmailAsync(email);
        if (user != null)
        {
            var newToken = await googleService.SignInUserWithExternal(user, info);
            if (newToken == null) return Redirect("/");

            return Redirect("/Account");
        }

        var token = await googleService.CreateUserWithExternal(email, info);
        if (token == null) return Problem("Error creating a new user.");

        return Redirect("/Account");
    }
}