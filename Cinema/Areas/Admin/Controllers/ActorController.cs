using AbsoluteCinema.Models;
using AbsoluteCinema.Repositories.IRepositories;
using AbsoluteCinema.ViewModels;
using AbsoluteCinema.Helper;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace AbsoluteCinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorController : Controller
    {
        private readonly IRepository<Actor> _repository;
        private readonly IRepository<Movie> _movieRepository;
        private readonly IFileUpload _fileUpload;

        public ActorController(IRepository<Actor> repository, IRepository<Movie> movieRepository, IFileUpload fileUpload)
        {
            _repository = repository;
            _movieRepository = movieRepository;
            _fileUpload = fileUpload;
        }

        // 1. Index
        [HttpGet]
        public IActionResult Index(string? query, int pageNumber = 1)
        {
            int pageSize = 5;
            var actors = _repository.Get(
                includes: new Expression<Func<Actor, object>>[]
                {
                    a => a.MovieActors
                }
            );

            if (!string.IsNullOrEmpty(query))
            {
                actors = actors.Where(c => c.Name.ToLower().Contains(query.ToLower()));
            }

            int totalItems = actors.Count();
            var pagedActors = actors.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var actorVMs = pagedActors.Select(c => new ActorDetailsVM
            {
                ActorId = c.Id,
                ActorName = c.Name,
                ProfilePicture = c.Img,
                MoviesCount = c.MovieActors?.Count ?? 0,
                Movies = c.MovieActors != null ? c.MovieActors.Select(m => new MovieVM
                {
                    Id = m.MovieId,
                    Title = m.Movie?.Name ?? string.Empty,
                    ExistingMainImg = m.Movie?.MainImg ?? string.Empty,
                    Description = m.Movie?.Description ?? string.Empty
                }).ToList() : new List<MovieVM>()
            }).ToList();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.SearchQuery = query;

            return View(actorVMs);
        }

        // 2. Details
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

            if (actor == null) return NotFound();

            var actorMovies = _movieRepository.Get()
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
                ProfilePicture = actor.Img,
                MoviesCount = actorMovies.Count,
                Movies = actorMovies
            };

            return View(actorDetailsVM);
        }

        // Get: Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new ActorVM());
        }

        // Post: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActorVM model)
        {
            if (!ModelState.IsValid) return View(model);

            string? imgPath = _fileUpload.SaveFile(model.ProfileImgFile, FileType.Img);

            var actor = new Actor
            {
                Name = model.Name,
                Img = imgPath ?? string.Empty
            };

            await _repository.CreateAsync(actor);
            await _repository.CommitAsync();

            TempData["success"] = "Actor created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // 4. Update (Get)
        [HttpGet]
        public IActionResult Update(int id)
        {
            var actor = _repository.GetOne(expression: a => a.Id == id);
            if (actor == null) return NotFound();

            var model = new ActorVM
            {
                Id = actor.Id,
                Name = actor.Name,
                ExistingProfileImg = actor.Img
            };

            return View(model);
        }

        // 4. Update (Post)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, ActorVM model)
        {
            var actor = _repository.GetOne(expression: a => a.Id == id);
            if (actor == null) return NotFound();

            if (!ModelState.IsValid)
            {
                model.ExistingProfileImg = actor.Img; 
            }

            string? updatedImg = _fileUpload.UpdateFile(model.ProfileImgFile, actor.Img, FileType.Img);

            actor.Name = model.Name;
            actor.Img = updatedImg ?? actor.Img;

            _repository.Update(actor);
            await _repository.CommitAsync();

            TempData["success"] = "Actor updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var actor =  _repository.Get().FirstOrDefault(i=>i.Id==id);
            if (actor == null)
            {
                return Json(new { success = false, message = "Actor not found!" });
            }

            // لو حابب تمسح الصورة القديمة من الملفات كمان
             _fileUpload.DeleteFileLocally(actor.Img);

            _repository.Delete(actor);
            await _repository.CommitAsync();

            return Json(new { success = true, message = "Actor deleted successfully!" });
        }
    }
}