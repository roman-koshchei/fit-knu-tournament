using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers;

public class RegisterController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Post(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match");
                return View(model);
            }
            // если пароли совпадают, сохранить пользователя, позже проделаем

            return RedirectToAction("Index", "Account");
        }
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> LoginPost()
    {
        return Ok();
    }
}