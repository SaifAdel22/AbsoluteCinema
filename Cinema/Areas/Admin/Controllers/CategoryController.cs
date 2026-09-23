using AbsoluteCinema.Services;
using AbsoluteCinema.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AbsoluteCinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public IActionResult Index(string query, int pageNumber = 1)
        {
            int pageSize = 5;
            var categories = _categoryService.GetPagedCategories(query, pageNumber, pageSize, out int totalItems);

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(decimal.Divide(totalItems, pageSize));
            ViewBag.Query = query;

            return View(categories);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var category = _categoryService.GetCategoryDetails(id);
            if (category == null) return NotFound();

            return View(category);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CategoryVM());
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryVM model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            await _categoryService.CreateCategoryAsync(model, ct);
            TempData["success"] = "Category Created Successfully!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet]
        public IActionResult Update(int id)
        {
            var category = _categoryService.GetCategoryById(id);
            if (category == null) return NotFound();

            return View(category);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CategoryVM model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            await _categoryService.UpdateCategoryAsync(model, ct);
            TempData["success"] = "Category Updated Successfully!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var result = await _categoryService.DeleteCategoryAsync(id, ct);
            if (!result) return NotFound();

            TempData["success"] = "Category Deleted Successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}