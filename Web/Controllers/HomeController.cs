using Data;
using Data.Tables;
using Lib;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web.Config;
using Web.Models;
using System.Threading.Tasks;

namespace Web.Controllers;

/// <summary>
/// Controller for handling home-related actions.
/// </summary>
public class HomeController : Controller
{
    private readonly UserManager<User> userManager;
    private readonly Jwt jwt;
    private readonly Db db;

    public HomeController(UserManager<User> userManager, Jwt jwt, Db db)
    {
        this.userManager = userManager;
        this.jwt = jwt;
        this.db = db;
    }

    /// <summary>
    /// Displays the home page with login information.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        bool userExist = false;
        var uid = User.MaybeUid();
        if (uid == null)
        {
            Response.Cookies.Delete("token");
        }
        else
        {
            userExist = await db.Users.Have(x => x.Id == uid);
            if (!userExist) Response.Cookies.Delete("token");
        }

        return View(new LoginViewModel(IsRegistered: userExist, Error: null));
    }

    /// <summary>
    /// Represents the request body for the login action.
    /// </summary>
    public record LoginInput(string Email, string Password);

    /// <summary>
    /// Handles user login and redirects to the account page on success.
    /// </summary>
    public async Task<IActionResult> Login(LoginInput input)
    {
        // Attempt to find the user by email.
        var user = await userManager.FindByEmailAsync(input.Email);
        if (user == null)
            return View("Index", new LoginViewModel(false, Error: "User with such email isn't found"));

        // Check if the provided password is correct.
        var passwordCorrect = await userManager.CheckPasswordAsync(user, input.Password);
        if (!passwordCorrect)
            return View("Index", new LoginViewModel(false, Error: "Password is incorrect"));

        // Generate a JWT token for the authenticated user.
        var token = jwt.Token(user.Id, user.Version);
        Response.AddAuthCookie(token);

        // Redirect to the account page.
        return RedirectToAction("Index", "Account");
    }

    /// <summary>
    /// Displays the error page.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
