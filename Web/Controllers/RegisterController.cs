﻿using Data.Tables;
using Lib;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Config;
using Web.Models;

namespace Web.Controllers;

public class RegisterController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly Jwt _jwt;

    public RegisterController(UserManager<User> userManager, Jwt jwtService)
    {
        _userManager = userManager;
        _jwt = jwtService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match");
                return BadRequest("Passwords do not match");
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Email already registered");
                return BadRequest("Email already registered");
            }

            var newUser = new User(model.Email);

            var result = await _userManager.CreateAsync(newUser, model.Password);
            if (!result.Succeeded)
            {
                // Handle identity errors
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return StatusCode(500, ModelState);
            }

            // User created successfully, generate authentication token
            var token = _jwt.Token(newUser.Id, newUser.Version);

            Response.AddAuthCookie(token);

            return Redirect("/Account");
        }
        return View("Index", new RegisterViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> LoginPost()
    {
        // Implement your login logic here
        // This method should handle user login
        return Ok();
    }
}