using AbsoluteCinema.Models;
using AbsoluteCinema.Repositories.IRepositories;
using AbsoluteCinema.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Linq.Expressions;

namespace AbsoluteCinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        public IRepository<Movie> _repository;
        public IRepository<Category> _categoryRepository;
        public IRepository<Cinema> _cinemaRepository;
        public IRepository<MovieSubImg> _subImgRepository;
        public IRepository<Actor> _ActorRepository;
        public IRepository<MovieActor > _movieactorRepository;

        public MovieController(IRepository<Movie> repository, IRepository<Category> categoryRepository, IRepository<Cinema> cinemaRepository, IRepository<MovieSubImg> subImgRepository , IRepository<Actor> ActorRepository , IRepository<MovieActor> movieactorRepository)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _cinemaRepository = cinemaRepository;
            _subImgRepository = subImgRepository;
            _ActorRepository = ActorRepository;
            _movieactorRepository = movieactorRepository;
        }

        // GET: MovieController/Index
        [HttpGet]
        public IActionResult Index(string searchTitle)
        {
            var movies = _repository.Get(
                expression: string.IsNullOrEmpty(searchTitle) ? null : m => m.Name.Contains(searchTitle),
                includes: new Expression<Func<Movie, object>>[] { m => m.Category },
                tracked: false
            );

            var moviesVM = movies.Select(m => new MovieVM
            {
                Id = m.Id,
                Title = m.Name,
                Price = m.Price,
                ExistingMainImg = m.MainImg,
                Status = m.Status,
                CategoryName = m.Category.Name
            }).ToList();

            return View(moviesVM);
        }

        // GET: MovieController/Details/5
        [HttpGet]
        public IActionResult Details(int id)
        {
            var movie = _repository.Get().FirstOrDefault(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            var subImages = GetMovieSubImages(id);

            var movieactors = _movieactorRepository.Get(expression : ma => ma.MovieId == id).Select(ac => new ActorVM
            {
                Id = ac.ActorId,
                Name =ac.Actor.Name
            });

            var movieVM = new MovieVM
            {
                Id = movie.Id,
                Title = movie.Name, 
                Description = movie.Description,
                Price = movie.Price,
                ExistingMainImg = movie.MainImg,
                ExistingSubImages = subImages,
                MovieActors = movieactors.ToList()
            };

            return View(movieVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var movie = _repository.GetOne(expression: i => i.Id == id);
            if (movie != null)
            {
                movie.Status = !movie.Status; 
                _repository.Update(movie);
               await _repository.CommitAsync();

            }
            return RedirectToAction(nameof(Index));
        }

        // POST: MovieController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MovieVM model)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MovieController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MovieController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, MovieVM model)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        // POST: MovieController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public List<MovieSubImgVM> GetMovieSubImages(int movieId)
        {
            var subImages = _subImgRepository.Get(expression: si => si.MovieId == movieId);

            if (subImages == null || !subImages.Any())
            {
                return new List<MovieSubImgVM>();
            }

            return subImages.Select(subImg => new MovieSubImgVM
            {
                Id = subImg.Id,
                Img = subImg.Img
            }).ToList();
        }

    }
}