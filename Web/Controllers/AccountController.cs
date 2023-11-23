using Data;
using Data.Tables;
using Humanizer;
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

    public AccountController(Db db, UserManager<User> userManager)
    {
        this.db = db;
        this.userManager = userManager;
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
}