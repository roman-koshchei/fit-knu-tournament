using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

public class LoginingController : Controller
{
    public IActionResult Index()

    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> LoginPost()
    {
        return Ok();
    }
}