using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Config;
using Web.Models;

namespace Web.Controllers;

public class AccountController : Controller
{
    private readonly Db db;

    public AccountController(Db db)
    {
        this.db = db;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var uid = User.Uid();

        var user = await db.Users
            .Select(x => new AccountViewModel(x.Id, x.Email))
            .QueryOne(x => x.Id == uid);

        return View(user);
    }

    [HttpGet]
    public async Task<IActionResult> Delete()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmDelete()
    {
    }
}