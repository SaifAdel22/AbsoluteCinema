using AbsoluteCinema.Data; // تأكد من استدعاء الـ Data namespace بتاعتك عشان الـ ApplicationDbContext
using AbsoluteCinema.Models;
using AbsoluteCinema.Repositories.IRepositories;
using AbsoluteCinema.ViewModels;
using AbsoluteCinema.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AbsoluteCinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context; // استخدام الـ DbContext مباشرة للربط الحقيقي
        public IRepository<Movie> _repository;
        public IRepository<Category> _categoryRepository;
        public IRepository<Cinema> _cinemaRepository;
        public IRepository<MovieSubImg> _subImgRepository;
        public IRepository<Actor> _ActorRepository;
        private readonly IFileUpload _fileUpload;

        public MovieController(
            ApplicationDbContext context,
            IRepository<Movie> repository,
            IRepository<Category> categoryRepository,
            IRepository<Cinema> cinemaRepository,
            IRepository<MovieSubImg> subImgRepository,
            IRepository<Actor> ActorRepository,
            IFileUpload fileUpload)
        {
            _context = context;
            _repository = repository;
            _categoryRepository = categoryRepository;
            _cinemaRepository = cinemaRepository;
            _subImgRepository = subImgRepository;
            _ActorRepository = ActorRepository;
            _fileUpload = fileUpload;
        }

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
                CategoryName = m.Category.Name ?? string.Empty
            }).ToList();

            return View(moviesVM);
        }

        [HttpGet]
        [HttpGet]
        public IActionResult Details(int id)
        {
            var movie = _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Include(m => m.SubImgs)
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                .FirstOrDefault(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            var movieVM = new MovieVM
            {
                Id = movie.Id,
                Title = movie.Name,
                Description = movie.Description,
                Price = movie.Price,
                StartDate = movie.DateTime,
                Status = movie.Status,
                ExistingMainImg = movie.MainImg,
                CategoryName = movie.Category?.Name ?? string.Empty,

                ExistingSubImages = movie.SubImgs.Select(si => new MovieSubImgVM { Id = si.Id, Img = si.Img }).ToList(),

                MovieActors = movie.MovieActors.Select(ma => new ActorVM
                {
                    Id = ma.Actor.Id,
                    Name = ma.Actor.Name,
                }).ToList()
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

        [HttpGet]
        public IActionResult Create()
        {
            var model = new MovieVM
            {
                Categories = _categoryRepository.Get().Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }),
                Cinemas = _cinemaRepository.Get().Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }),
                Actors = _ActorRepository.Get().Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name })
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieVM model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = _categoryRepository.Get().Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name });
                model.Cinemas = _cinemaRepository.Get().Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name });
                model.Actors = _ActorRepository.Get().Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name });
                return View(model);
            }

            string? mainImgPath = _fileUpload.SaveFile(model.MainImg, FileType.MovieMain);

            var movie = new Movie
            {
                Name = model.Title,
                Description = model.Description ?? string.Empty,
                Price = model.Price,
                DateTime = model.StartDate,
                CategoryId = model.CategoryId,
                CinemaId = model.CinemaId,
                MainImg = mainImgPath ?? string.Empty,
                Status = model.Status
            };

            // 1. إضافة الفيلم للـ DbContext الموحد
            await _context.Movies.AddAsync(movie);
            await _context.SaveChangesAsync(); // بيحفظ الفيلم ويدينا الـ ID فوراً

            // 2. حفظ الصور الفرعية إن وجدت تحت نفس الـ Context
            if (model.NewSubImages != null && model.NewSubImages.Any())
            {
                foreach (var subImg in model.NewSubImages)
                {
                    string? subImgPath = _fileUpload.SaveFile(subImg, FileType.MovieSub);
                    if (!string.IsNullOrEmpty(subImgPath))
                    {
                        await _context.MovieSubImgs.AddAsync(new MovieSubImg
                        {
                            MovieId = movie.Id,
                            Img = subImgPath
                        });
                    }
                }
            }

            // 3. حفظ الممثلين المختارين تحت نفس الـ Context (مستحيل تضيع هنا)
            if (model.SelectedActorIds != null && model.SelectedActorIds.Any())
            {
                foreach (var actorId in model.SelectedActorIds)
                {
                    await _context.MovieActors.AddAsync(new MovieActor
                    {
                        MovieId = movie.Id,
                        ActorId = actorId,
                        CharacterName = "Default"
                    });
                }
            }

            // الحفظ النهائي لكل العلاقات (صور وممثلين) دفعة واحدة
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var movie = _context.Movies
                .Include(m => m.MovieActors)
                .Include(m => m.SubImgs) // <-- تأكد من وجود هذه الـ Include
                .FirstOrDefault(m => m.Id == id);

            if (movie == null) return NotFound();

            var selectedActorIds = movie.MovieActors.Select(ma => ma.ActorId).ToList();

            var model = new MovieVM
            {
                Id = movie.Id,
                Title = movie.Name,
                Description = movie.Description,
                Price = movie.Price,
                StartDate = movie.DateTime,
                CategoryId = movie.CategoryId,
                CinemaId = movie.CinemaId,
                Status = movie.Status,
                ExistingMainImg = movie.MainImg,
                SelectedActorIds = selectedActorIds,

                // **هذا السطر هو الناقص والذي سيقوم بعرض الصور الفرعية الحالية:**
                ExistingSubImages = movie.SubImgs.Select(si => new MovieSubImgVM
                {
                    Id = si.Id,
                    Img = si.Img
                }).ToList(),

                Categories = _categoryRepository.Get().Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }),
                Cinemas = _cinemaRepository.Get().Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }),
                Actors = _ActorRepository.Get().Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name })
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, MovieVM model)
        {
            var movie = _context.Movies
                .Include(m => m.MovieActors)
                .Include(m => m.SubImgs)
                .FirstOrDefault(m => m.Id == id);

            if (movie == null) return NotFound();

            if (!ModelState.IsValid)
            {
                model.Categories = _categoryRepository.Get().Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name });
                model.Cinemas = _cinemaRepository.Get().Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name });
                model.Actors = _ActorRepository.Get().Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name });
                return View(model);
            }

            string? updatedMainImg = _fileUpload.UpdateFile(model.MainImg, model.ExistingMainImg, FileType.MovieMain);

            movie.Name = model.Title;
            movie.Description = model.Description ?? string.Empty;
            movie.Price = model.Price;
            movie.DateTime = model.StartDate;
            movie.CategoryId = model.CategoryId;
            movie.CinemaId = model.CinemaId;
            movie.Status = model.Status;
            movie.MainImg = updatedMainImg ?? movie.MainImg;

            // مسح الممثلين القدامى وإضافة الجداد في نفس السياق
            _context.MovieActors.RemoveRange(movie.MovieActors);

            if (model.SelectedActorIds != null && model.SelectedActorIds.Any())
            {
                foreach (var actorId in model.SelectedActorIds)
                {
                    await _context.MovieActors.AddAsync(new MovieActor
                    {
                        MovieId = id,
                        ActorId = actorId,
                        CharacterName = "Default"
                    });
                }
            }

            if (model.NewSubImages != null && model.NewSubImages.Any())
            {
                foreach (var subImg in model.NewSubImages)
                {
                    string? subImgPath = _fileUpload.SaveFile(subImg, FileType.MovieSub);
                    if (!string.IsNullOrEmpty(subImgPath))
                    {
                        await _context.MovieSubImgs.AddAsync(new MovieSubImg { MovieId = id, Img = subImgPath });
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet, HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = _repository.GetOne(expression: m => m.Id == id);
            if (movie == null)
            {
                TempData["error"] = "Movie not found!";
                return NotFound();
            }

            if (!string.IsNullOrEmpty(movie.MainImg))
            {
                var relativePath = movie.MainImg.TrimStart('/', '\\');
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

                _fileUpload.DeleteFileLocally(fullPath);
            }

            var subImages = _subImgRepository.Get(si => si.MovieId == id).ToList();
            if (subImages.Any())
            {
                foreach (var subImg in subImages)
                {
                    if (!string.IsNullOrEmpty(subImg.Img))
                    {
                        var subRelativePath = subImg.Img.TrimStart('/', '\\');
                        string subPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", subRelativePath);

                        _fileUpload.DeleteFileLocally(subPath);
                    }
                }

                if (_subImgRepository is IBulkRepository<MovieSubImg> bulkSubImgRepo)
                {
                    bulkSubImgRepo.DeleteRange(subImages);
                }
            }

            _repository.Delete(movie);
            await _repository.CommitAsync();

            TempData["success"] = "Movie deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost,HttpGet]
        public async Task<IActionResult> DeleteSubImage(int id, int movieId)
        {
            var subImg = _subImgRepository.GetOne(s => s.Id == id);
            if (subImg != null)
            {
                if (!string.IsNullOrEmpty(subImg.Img))
                {
                    var relativePath = subImg.Img.TrimStart('/', '\\');
                    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);
                    _fileUpload.DeleteFileLocally(fullPath);
                }

                _subImgRepository.Delete(subImg);
                await _subImgRepository.CommitAsync();

                TempData["success"] = "Sub image deleted successfully!";
            }

            return RedirectToAction(nameof(Update), new { id = movieId });
        }
    }
}