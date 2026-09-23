using Microsoft.AspNetCore.Mvc;

namespace AbsoluteCinema.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}