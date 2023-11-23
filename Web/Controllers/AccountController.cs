using Data;
using Data.Tables;
using Lib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Config;
using Web.Models;

namespace Web.Controllers;

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

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var uid = User.Uid();

        var user = await db.Users.QueryOne(x => x.Id == uid);
        if (user == null) return RedirectToAction("NotFoundPage", "Home");

        return View(new AccountViewModel(user.Id, user.Email));
    }

    [HttpGet]
    public IActionResult Delete()
    {
        return View(new DeleteViewModel(new List<string>()));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ConfirmDelete()
    {
        var uid = User.Uid();

        var user = await db.Users.QueryOne(x => x.Id == uid);
        if (user == null) return Redirect("/");

        var result = await userManager.DeleteAsync(user);
        if (result.Succeeded) return Redirect("/");

        return View("Delete", new DeleteViewModel(result.Errors.Select(x => x.Description)));
    }

    [HttpGet]
    [Authorize]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("token");
        return Redirect("/");
    }

    [HttpGet]
    [Authorize]
    public IActionResult Email()
    {
        return View();
    }

    public record EmailBody(string Email);

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

    [HttpGet]
    [Authorize]
    public IActionResult Password()
    {
        return View("Password", new ChangePasswordViewModel(new List<string>()));
    }

    public record PasswordBody(string OldPassword, string NewPassword);

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