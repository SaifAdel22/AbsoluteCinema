using AbsoluteCinema.Services;
using AbsoluteCinema.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AbsoluteCinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]

    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet]
        public IActionResult Index(string searchTitle, int pageNumber = 1)
        {
            int pageSize = 5;
            var moviesVM = _movieService.GetPagedMovies(searchTitle, pageNumber, pageSize, out int totalItems);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(decimal.Divide(totalItems, pageSize));
            ViewBag.SearchTitle = searchTitle;

            return View(moviesVM);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var movieVM = await _movieService.GetMovieDetailsAsync(id);
            if (movieVM == null)
            {
                return NotFound();
            }

            return View(movieVM);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            await _movieService.ToggleStatusAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        public IActionResult Create()
        {
            var model = new MovieVM();
            _movieService.PopulateDropdowns(model);
            return View(model);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieVM model)
        {
            if (!ModelState.IsValid)
            {
                _movieService.PopulateDropdowns(model);
                return View(model);
            }

            await _movieService.CreateMovieAsync(model);
            TempData["success"] = "Movie Added successfully!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var model = await _movieService.GetMovieForUpdateAsync(id);
            if (model == null) return NotFound();

            return View(model);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, MovieVM model)
        {
            if (!ModelState.IsValid)
            {
                _movieService.PopulateDropdowns(model);
                return View(model);
            }

            await _movieService.UpdateMovieAsync(id, model);
            TempData["success"] = "Movie Updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _movieService.DeleteMovieAsync(id);
            if (!result) return NotFound();

            TempData["success"] = "Movie Deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> DeleteSubImage(int subImgId, int movieId)
        {
            await _movieService.DeleteSubImageAsync(subImgId);

            return RedirectToAction(nameof(Update), new { id = movieId });
        }
    }
}