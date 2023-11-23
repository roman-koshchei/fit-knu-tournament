using Data;
using Data.Tables;
using Lib;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web.Config;
using Web.Models;

namespace Web.Controllers;

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

    public async Task<IActionResult> Index()
    {
        return View(new LoginViewModel(IsRegistered: User.HaveUid(), Error: null));
    }

    public record LoginInput(string Email, string Password);

    public async Task<IActionResult> Login(LoginInput input)
    {
        var user = await userManager.FindByEmailAsync(input.Email);
        if (user == null) return View("Index", new LoginViewModel(false, Error: "User with such email isn't found"));

        var passwordCorrect = await userManager.CheckPasswordAsync(user, input.Password);
        if (!passwordCorrect) return View("Index", new LoginViewModel(false, Error: "Password is incorrect"));

        var token = jwt.Token(user.Id, user.Version);
        Response.AddAuthCookie(token);

        return RedirectToAction("Index", "Account");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}