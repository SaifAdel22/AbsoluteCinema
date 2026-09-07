using Microsoft.AspNetCore.Mvc;

namespace AbsoluteCinema.Areas.Admin.Controllers
{
    public class MovieController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
