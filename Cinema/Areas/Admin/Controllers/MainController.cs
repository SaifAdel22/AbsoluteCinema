using AbsoluteCinema.Models;
using AbsoluteCinema.Repositories.UnitOfWork;
using AbsoluteCinema.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AbsoluteCinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MainController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public MainController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Statistics()
        {
            var allMovies = _unitOfWork.movieRepository.Get();

            var statsVM = new StatisticsVM
            {
                MoviesCount = allMovies.Count(),
                ActiveMoviesCount = allMovies.Count(m => m.Status),
                InactiveMoviesCount = allMovies.Count(m => !m.Status),
                ActorsCount = _unitOfWork.actorRepository.Get().Count(),
                CinemasCount = _unitOfWork.cinemaRepository.Get().Count(),
                CategoriesCount = _unitOfWork.categoryRepository.Get().Count()
            };

            return View(statsVM);
        }
    }
}