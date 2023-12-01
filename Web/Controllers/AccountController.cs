using Data;
using Data.Tables;
using Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Config;
using Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Controllers;

/// <summary>
/// Controller for handling user account-related actions.
/// </summary>
public class AccountController : Controller
{
    private readonly Db db;
    private readonly UserManager<User> userManager;
    private readonly Jwt jwt;

    public AccountController(Db db, UserManager<User> userManager, Jwt jwt)
    {
        this.db = db;
        this.userManager = userManager;
        this.jwt = jwt;
    }

    /// <summary>
    /// Displays the account details for the authenticated user.
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var uid = User.Uid();

        var user = await db.Users.QueryOne(x => x.Id == uid);
        if (user == null) return RedirectToAction("NotFoundPage", "Home");

        return View(new AccountViewModel(user.Id, user.Email, user.PasswordHash == null));
    }

    /// <summary>
    /// Displays the confirmation view for account deletion.
    /// </summary>
    [HttpGet]
    public IActionResult Delete()
    {
        return View(new DeleteViewModel(new List<string>()));
    }

    /// <summary>
    /// Confirms and processes the deletion of the authenticated user's account.
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ConfirmDelete()
    {
        var uid = User.Uid();

        var user = await db.Users.QueryOne(x => x.Id == uid);
        if (user == null) return Redirect("/");

        var result = await userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            Response.Cookies.Delete("token");
            return Redirect("/");
        }

        return View("Delete", new DeleteViewModel(result.Errors.Select(x => x.Description)));
    }

    /// <summary>
    /// Logs out the authenticated user and redirects to the home page.
    /// </summary>
    [HttpGet]
    [Authorize]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("token");
        return Redirect("/");
    }

    /// <summary>
    /// Displays the view for changing the user's email.
    /// </summary>
    [HttpGet]
    [Authorize]
    public IActionResult Email()
    {
        return View();
    }

    /// <summary>
    /// Represents the request body for changing the user's email.
    /// </summary>
    public record EmailBody(string Email);

    /// <summary>
    /// Changes the user's email and updates the authentication token.
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ChangeEmail(EmailBody body)
    {
        var uid = User.Uid();

        var user = await db.Users.QueryOne(x => x.Id == uid);
        if (user == null) return Redirect("/");

        user.Email = body.Email;
        user.NormalizedEmail = body.Email.ToUpper();
        user.Version += 1;

        var saved = await db.Save();
        if (!saved) return Redirect("/Account");

        var token = jwt.Token(user.Id, user.Version);
        Response.AddAuthCookie(token);

        return Redirect("/Account");
    }

    /// <summary>
    /// Displays the view for changing the user's password.
    /// </summary>
    [HttpGet]
    [Authorize]
    public IActionResult Password()
    {
        return View("Password", new ChangePasswordViewModel(new List<string>()));
    }

    /// <summary>
    /// Represents the request body for changing the user's password.
    /// </summary>
    public record PasswordBody(string OldPassword, string NewPassword);

    /// <summary>
    /// Changes the user's password and updates the authentication token.
    /// </summary>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ChangePassword(PasswordBody body)
    {
        var uid = User.Uid();

        var user = await db.Users.QueryOne(x => x.Id == uid);
        if (user == null) return Redirect("/");

        var result = await userManager.ChangePasswordAsync(user, body.OldPassword, body.NewPassword);
        if (!result.Succeeded) return View("Password",
            new ChangePasswordViewModel(result.Errors.Select(x => x.Description))
        );

        user.Version += 1;
        var saved = await db.Save();
        if (!saved) return Redirect("/Account");

        var token = jwt.Token(user.Id, user.Version);
        Response.AddAuthCookie(token);

        return Redirect("/Account");
    }
}