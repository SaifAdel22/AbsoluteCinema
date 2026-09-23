using AbsoluteCinema.Services;
using AbsoluteCinema.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AbsoluteCinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class ActorController : Controller
    {
        private readonly IActorService _actorService;

        public ActorController(IActorService actorService)
        {
            _actorService = actorService;
        }

        [HttpGet]
        public IActionResult Index(string query, int pageNumber = 1)
        {
            int pageSize = 5;
            var actors = _actorService.GetPagedActors(query, pageNumber, pageSize, out int totalItems);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(decimal.Divide(totalItems, pageSize));
            ViewBag.Query = query;

            return View(actors);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var actor = _actorService.GetActorDetails(id);
            if (actor == null) return NotFound();

            return View(actor);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new ActorVM());
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActorVM model)
        {
            if (!ModelState.IsValid) return View(model);

            await _actorService.CreateActorAsync(model);
            TempData["success"] = "Actor Created Successfully!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        public IActionResult Update(int id)
        {
            var actor = _actorService.GetActorDetails(id);
            if (actor == null) return NotFound();

            var model = new ActorVM
            {
                Id = actor.ActorId,
                Name = actor.ActorName,
                ExistingProfileImg = actor.ProfilePicture
            };

            return View(model);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, ActorVM model)
        {
            if (!ModelState.IsValid) return View(model);

            await _actorService.UpdateActorAsync(id, model);
            TempData["success"] = "Actor Updated Successfully!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _actorService.DeleteActorAsync(id);
                return Json(new { success = true, message = "Actor Deleted Successfully!" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Failed to delete the actor." });
            }
        }
    }
}