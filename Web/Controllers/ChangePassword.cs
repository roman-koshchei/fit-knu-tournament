using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class ChangePassword : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}