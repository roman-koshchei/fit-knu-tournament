using Data;
using Data.Tables;
using Lib;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Config;
using Web.Models;
using System.Threading.Tasks;

namespace Web.Controllers;

/// <summary>
/// Controller for handling user registration-related actions.
/// </summary>
public class RegisterController : Controller
{
    private readonly UserManager<User> userManager;
    private readonly Jwt jwt;
    private readonly Db db;

    public RegisterController(UserManager<User> userManager, Jwt jwt, Db db)
    {
        this.userManager = userManager;
        this.jwt = jwt;
        this.db = db;
    }

    /// <summary>
    /// Displays the registration page for new users.
    /// </summary>
    public IActionResult Index()
    {
        var haveUid = User.HaveUid();
        if (haveUid) return Redirect("Account");

        return View(new RegisterViewModel());
    }

    /// <summary>
    /// Handles user registration and redirects to the account page on success.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        // Check if the model is valid.
        if (!ModelState.IsValid) return View("Index", new RegisterViewModel());

        // Check if the password and confirm password match.
        if (model.Password != model.ConfirmPassword)
        {
            ModelState.AddModelError("ConfirmPassword", "Passwords do not match");
            return BadRequest("Passwords do not match");
        }

        // Check if the email is already registered.
        var existingUser = await userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            ModelState.AddModelError("Email", "Email already registered");
            return BadRequest("Email already registered");
        }

        // Create a new user.
        var newUser = new User(model.Email);

        // Attempt to create the user in the identity system.
        var result = await userManager.CreateAsync(newUser, model.Password);
        if (!result.Succeeded)
        {
            // Handle identity errors
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View("Index", new RegisterViewModel());
        }

        // User created successfully, generate authentication token.
        var token = jwt.Token(newUser.Id, newUser.Version);

        // Add the authentication token to the response.
        Response.AddAuthCookie(token);

        // Redirect to the account page.
        return Redirect("/Account");
    }
}
