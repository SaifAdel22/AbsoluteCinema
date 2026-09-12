using AbsoluteCinema.Helper;
using AbsoluteCinema.Models;
using AbsoluteCinema.Repositories.IRepositories;
using AbsoluteCinema.Repositories.UnitOfWork;
using AbsoluteCinema.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AbsoluteCinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileUpload _fileUpload;

        public CinemaController(IUnitOfWork unitOfWork, IFileUpload fileUpload)
        {
            _unitOfWork = unitOfWork;
            _fileUpload = fileUpload;
        }

        [HttpGet]
        public IActionResult Index(string? query, int pageNumber = 1)
        {
            int pageSize = 5;
            var cinemas = _unitOfWork.cinemaRepository.Get();

            if (!string.IsNullOrEmpty(query))
            {
                cinemas = cinemas.Where(c => c.Name.ToLower().Contains(query.ToLower()));
            }

            int totalItems = cinemas.Count();
            var pagedCinemas = cinemas.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var cinemaVMs = pagedCinemas.Select(c => new CinemaVM
            {
                Id = c.Id,
                Name = c.Name,
                ExistingCinemaLogo = c.Img,
            }).ToList();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.SearchQuery = query;

            return View(cinemaVMs);
        }

        public IActionResult Details(int id)
        {
            var cinema = _unitOfWork.cinemaRepository.GetOne(c => c.Id == id);
            var cinemamovies = _unitOfWork.movieRepository.Get(expression: m => m.CinemaId == id).Select(m => new MovieVM
            {
                Id = m.Id,
                Title = m.Name,
                ExistingMainImg = m.MainImg,
                Price = m.Price,
                Description = m.Description
            }).ToList();

            var cinemaVM = new CinemaVM
            {
                Id = cinema.Id,
                Name = cinema.Name,
                ExistingCinemaLogo = cinema.Img,
                Movies = cinemamovies
            };

            return View(cinemaVM);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var cinemaVM = new CinemaVM();
            return View(cinemaVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CinemaVM cinemaVM)
        {
            if (!ModelState.IsValid)
            {
                return View(cinemaVM);
            }

            if (cinemaVM.CinemaLogo == null || cinemaVM.CinemaLogo.Length == 0)
            {
                ModelState.AddModelError("CinemaLogo", "Cinema logo is required.");
                return View(cinemaVM);
            }

            if (cinemaVM.CinemaLogo.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("CinemaLogo", "Image size exceeds the 2MB limit.");
                return View(cinemaVM);
            }

            if (_unitOfWork.cinemaRepository.GetOne(c => c.Name.ToLower() == cinemaVM.Name.ToLower()) != null)
            {
                ModelState.AddModelError("Name", "Cinema name already exists.");
                return View(cinemaVM);
            }

            string? logoPath = null;
            if (cinemaVM.CinemaLogo != null)
            {
                logoPath = _fileUpload.SaveFile(cinemaVM.CinemaLogo, FileType.Img);
            }

            var cinema = new Cinema
            {
                Name = cinemaVM.Name,
                Img = logoPath ?? string.Empty
            };

            await _unitOfWork.cinemaRepository.CreateAsync(cinema);
            await _unitOfWork.cinemaRepository.CommitAsync();

            TempData["success"] = "Cinema created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var cinema = _unitOfWork.cinemaRepository.GetOne(c => c.Id == id);
            if (cinema == null)
            {
                return NotFound();
            }
            var cinemaVM = new CinemaVM
            {
                Id = cinema.Id,
                Name = cinema.Name,
                ExistingCinemaLogo = cinema.Img
            };
            return View(cinemaVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, CinemaVM cinemaVM)
        {
            var cinema = _unitOfWork.cinemaRepository.GetOne(c => c.Id == id);
            if (cinema == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                cinemaVM.ExistingCinemaLogo = cinema.Img;
                return View(cinemaVM);
            }
            if (cinemaVM.CinemaLogo != null && cinemaVM.CinemaLogo.Length > 2 * 1024 * 1024)
            {
                ModelState.AddModelError("CinemaLogo", "Image size exceeds the 2MB limit.");
                cinemaVM.ExistingCinemaLogo = cinema.Img;
                return View(cinemaVM);
            }
            if (_unitOfWork.cinemaRepository.GetOne(c => c.Name.ToLower() == cinemaVM.Name.ToLower() && c.Id != id) != null)
            {
                ModelState.AddModelError("Name", "Cinema name already exists.");
                cinemaVM.ExistingCinemaLogo = cinema.Img;
                return View(cinemaVM);
            }
            string? logoPath = cinema.Img;
            if (cinemaVM.CinemaLogo != null)
            {
                logoPath = _fileUpload.SaveFile(cinemaVM.CinemaLogo, FileType.Img);
            }
            cinema.Name = cinemaVM.Name;
            cinema.Img = logoPath ?? string.Empty;
            _unitOfWork.cinemaRepository.Update(cinema);
            await _unitOfWork.cinemaRepository.CommitAsync();
            TempData["success"] = "Cinema updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var cinema = _unitOfWork.cinemaRepository.GetOne(c => c.Id == id);
            if (cinema == null)
            {
                return Json(new { success = false, message = "Cinema not found!" });
            }

            if (!string.IsNullOrEmpty(cinema.Img))
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", cinema.Img.TrimStart('/'));
                _fileUpload.DeleteFileLocally(fullPath);
            }

            _unitOfWork.cinemaRepository.Delete(cinema);
            await _unitOfWork.cinemaRepository.CommitAsync();

            return Json(new { success = true, message = "Cinema deleted successfully!" });
        }
    }
}