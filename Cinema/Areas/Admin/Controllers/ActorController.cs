using AbsoluteCinema.Models;
using AbsoluteCinema.Repositories.IRepositories;
using AbsoluteCinema.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace AbsoluteCinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorController : Controller
    {

        public IRepository<Actor> _repository;
        public IRepository<Movie> _Movierepository;
        public ActorController(IRepository<Actor> repository, IRepository<Movie> movierepository)
        {
            _repository = repository;
            _Movierepository = movierepository;
        }
        [HttpGet]
        [HttpGet]
        public IActionResult Details(int id)
        {
            var actor = _repository.GetOne(
                expression: c => c.Id == id,
                includes: new Expression<Func<Actor, object>>[]
                {
            c => c.MovieActors
                }
            );

            if (actor == null)
            {
                return NotFound();
            }

            var actorMovies = _Movierepository.Get()
                .Where(m => m.MovieActors.Any(ma => ma.ActorId == id))
                .Select(m => new MovieVM
                {
                    Id = m.Id,
                    Title = m.Name,
                    ExistingMainImg = m.MainImg,
                    Price = m.Price,
                    Description = m.Description
                }).ToList();

            var actorDetailsVM = new ActorDetailsVM
            {
                ActorId = actor.Id,
                ActorName = actor.Name,
                bio = actorMovies.Count, 
                ProfilePicture = actor.Img,
                Movies = actorMovies
            };

            return View(actorDetailsVM);
        }



        // GET: ActorController
        [HttpGet]
        public IActionResult Index(string? query, int pageNumber = 1)
        {
            int pageSize = 5;
            var actor = _repository.Get();

            if (!string.IsNullOrEmpty(query))
            {
                actor = actor.Where(c => c.Name.ToLower().Contains(query.ToLower()));
            }

            int totalItems = actor.Count();
            var pagedActors = actor.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var actorVMs = pagedActors.Select(c => new ActorDetailsVM
            {
                ActorId = c.Id,
                ActorName = c.Name,
                ProfilePicture = c.Img,
                Movies = c.MovieActors.Select(m => new MovieVM
                {
                    Id = m.MovieId,
                    Title = m.Movie.Name,
                    ExistingMainImg = m.Movie.MainImg,
                    Description = m.Movie.Description
                }).ToList()
            }).ToList();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.SearchQuery = query;

            return View(actorVMs);
        }



        // GET: ActorController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ActorController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: ActorController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ActorController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: ActorController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ActorController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
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
    }
}
