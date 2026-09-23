using AbsoluteCinema.Services;
using AbsoluteCinema.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AbsoluteCinema.Areas.Customer.Controllers
{
    [Area(AreaConstants.CUSTOMER_AREA)]


    public class CinemaController : Controller
    {
        private readonly ICinemaServices _cinemaService;

        public CinemaController(ICinemaServices cinemaService)
        {
            _cinemaService = cinemaService;
        }

        [HttpGet]
        public IActionResult Index(string query, int pageNumber = 1)
        {
            int pageSize = 5;
            var cinemas = _cinemaService.GetPagedCinemas(query, pageNumber, pageSize, out int totalItems);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(decimal.Divide(totalItems, pageSize));
            ViewBag.Query = query;

            return View(cinemas);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var cinema = _cinemaService.GetCinemaDetails(id);
            if (cinema == null) return NotFound();

            return View(cinema);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CinemaVM());
        }

       
        
    }
}