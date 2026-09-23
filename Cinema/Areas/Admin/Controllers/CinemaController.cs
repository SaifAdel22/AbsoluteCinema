using AbsoluteCinema.Services;
using AbsoluteCinema.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AbsoluteCinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]

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

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CinemaVM model)
        {
            if (_cinemaService.IsCinemaNameExists(model.Name))
            {
                ModelState.AddModelError("Name", "Cinema name already exists.");
            }

            if (!ModelState.IsValid) return View(model);

            await _cinemaService.CreateCinemaAsync(model);
            TempData["success"] = "Cinema Created Successfully!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        public IActionResult Update(int id)
        {
            var cinema = _cinemaService.GetCinemaDetails(id);
            if (cinema == null) return NotFound();

            return View(cinema);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, CinemaVM model)
        {
            if (_cinemaService.IsCinemaNameExists(model.Name, id))
            {
                ModelState.AddModelError("Name", "Cinema name already exists.");
            }

            if (!ModelState.IsValid) return View(model);

            var result = await _cinemaService.UpdateCinemaAsync(id, model);
            if (!result) return NotFound();

            TempData["success"] = "Cinema Updated Successfully!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Delete(int id)
        {
            

            try
            {
                await _cinemaService.DeleteCinemaAsync(id);
                return Json(new { success = true, message = "Actor Deleted Successfully!" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Failed to delete the actor." });
            }
            return RedirectToAction(nameof(Index));


        }
    }
}