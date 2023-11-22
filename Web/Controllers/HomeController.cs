﻿using Data.Tables;
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

    public HomeController(UserManager<User> userManager, Jwt jwt)
    {
        this.userManager = userManager;
        this.jwt = jwt;
    }

    [HttpGet("/")]
    public IActionResult Index()
    {
        return View();
    }

    [NonAction]
    public void AddAuthCookie(string token)
    {
        Response.Cookies.Append("token", token, new()
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTimeOffset.Now.AddDays(30)
        });
    }

    public record LoginInput(string Email, string Password);

    [HttpPost("/")]
    public async Task<IActionResult> Login(LoginInput input)
    {
        var user = await userManager.FindByEmailAsync(input.Email);
        if (user == null) return NotFound();

        var passwordCorrect = await userManager.CheckPasswordAsync(user, input.Password);
        if (!passwordCorrect) return BadRequest();

        var token = jwt.Token(user.Id, user.Version);
        AddAuthCookie(token);

        return RedirectToAction("Index", "Account");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}