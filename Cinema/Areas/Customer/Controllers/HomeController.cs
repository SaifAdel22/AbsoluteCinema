using AbsoluteCinema.Services;
using AbsoluteCinema.Utility;
using Microsoft.AspNetCore.Mvc;

namespace AbsoluteCinema.Areas.Customer.Controllers
{
    [Area(AreaConstants.CUSTOMER_AREA)]
    public class HomeController : Controller
    {
        private readonly IMovieService _movieService;

        public HomeController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet]
        public IActionResult Index(string searchTitle, int pageNumber = 1)
        {
            int pageSize = 5;
            var moviesVM = _movieService.GetCustomerPagedMovies(searchTitle, pageNumber, pageSize, out int totalItems);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(decimal.Divide(totalItems, pageSize));
            ViewBag.SearchTitle = searchTitle;

            return View(moviesVM);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var movieVM = _movieService.GetCustomerMovieDetails(id);
            if (movieVM == null)
            {
                return NotFound();
            }

            return View(movieVM);
        }
    }
}