﻿using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}