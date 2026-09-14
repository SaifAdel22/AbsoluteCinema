using AbsoluteCinema.Utility;
using Microsoft.AspNetCore.Mvc;

namespace AbsoluteCinema.Areas.Customer.Controllers
{

    [Area (AreaConstants.CUSTOMER_AREA)]
    //        public const string CUSTOMER_AREA = "Customer";

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
