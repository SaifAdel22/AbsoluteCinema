using AbsoluteCinema.Data;
using AbsoluteCinema.Models;
using AbsoluteCinema.Repositories.IRepositories;
using AbsoluteCinema.Repositories.UnitOfWork;
using AbsoluteCinema.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AbsoluteCinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var category = _unitOfWork.categoryRepository.GetOne(c => c.Id == id);

            if (category is null)
            {
                TempData["error"] = "Category not found!";
                return RedirectToAction(nameof(Index));
            }

            var categoryDetailsVM = new CategoryDetailsVM
            {
                CategoryId = category.Id,
                CategoryName = category.Name,
                Movies = _unitOfWork.movieRepository.Get(m => m.CategoryId == id).Select(m => new MovieVM
                {
                    Id = m.Id,
                    Title = m.Name,
                    Description = m.Description,
                    Price = m.Price,
                    StartDate = m.DateTime,
                    ExistingMainImg = m.MainImg
                }).ToList()
            };

            return View(categoryDetailsVM);
        }

        [HttpGet]
        public IActionResult Index(string? query, int pageNumber = 1)
        {
            int pageSize = 5;
            var categories = _unitOfWork.categoryRepository.Get();

            if (!string.IsNullOrEmpty(query))
            {
                categories = categories.Where(c => c.Name.ToLower().Contains(query.ToLower()));
            }

            int totalItems = categories.Count();
            var pagedCategories = categories.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var categoryVMs = pagedCategories.Select(c => new CategoryVM
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.SearchQuery = query;

            return View(categoryVMs);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var categoryVM = new CategoryVM();
            return View(categoryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryVM categoryVM, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return View(categoryVM);

            var category = new Category
            {
                Name = categoryVM.Name
            };

            await _unitOfWork.categoryRepository.CreateAsync(category, ct);
            await _unitOfWork.categoryRepository.CommitAsync(ct);

            TempData["success"] = "Category created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var category = _unitOfWork.categoryRepository.GetOne(e => e.Id == id, tracked: false);

            if (category is null)
            {
                TempData["error"] = "Category not found!";
                return RedirectToAction(nameof(Index));
            }

            var categoryVM = new CategoryVM
            {
                Id = category.Id,
                Name = category.Name
            };

            return View(categoryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CategoryVM categoryVM, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return View(categoryVM);

            var categoryInDB = _unitOfWork.categoryRepository.GetOne(e => e.Id == categoryVM.Id, tracked: false);

            if (categoryInDB is null)
            {
                TempData["error"] = "Category not found!";
                return RedirectToAction(nameof(Index));
            }

            var category = new Category
            {
                Id = categoryVM.Id,
                Name = categoryVM.Name
            };

            _unitOfWork.categoryRepository.Update(category);
            await _unitOfWork.categoryRepository.CommitAsync(ct);

            TempData["success"] = "Category updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            var category = _unitOfWork.categoryRepository.GetOne(e => e.Id == id);

            if (category is null)
            {
                TempData["error"] = "Category not found!";
                return RedirectToAction(nameof(Index));
            }

            _unitOfWork.categoryRepository.Delete(category);
            await _unitOfWork.categoryRepository.CommitAsync(ct);

            TempData["success"] = "Category deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}