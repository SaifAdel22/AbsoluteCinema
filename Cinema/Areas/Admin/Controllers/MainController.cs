using AbsoluteCinema.Models;
using AbsoluteCinema.Repositories.IRepositories;
using AbsoluteCinema.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AbsoluteCinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MainController : Controller
    {
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<Actor> _actorRepository;
        private readonly IRepository<Cinema> _cinemaRepository;
        private readonly IRepository<Category> _categoryRepository;

        public MainController(
            IRepository<Movie> movieRepository,
            IRepository<Actor> actorRepository,
            IRepository<Cinema> cinemaRepository,
            IRepository<Category> categoryRepository)
        {
            _movieRepository = movieRepository;
            _actorRepository = actorRepository;
            _cinemaRepository = cinemaRepository;
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public IActionResult Statistics()
        {
            var allMovies = _movieRepository.Get();

            var statsVM = new StatisticsVM

            {
                MoviesCount = allMovies.Count(),
                ActiveMoviesCount = allMovies.Count(m => m.Status),
                InactiveMoviesCount = allMovies.Count(m => !m.Status),
                ActorsCount = _actorRepository.Get().Count(),
                CinemasCount = _cinemaRepository.Get().Count(),
                CategoriesCount = _categoryRepository.Get().Count()
            };

            return View(statsVM); 
        }
    }
}