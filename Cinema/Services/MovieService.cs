using AbsoluteCinema.Helper;
using AbsoluteCinema.Models;
using AbsoluteCinema.Repositories.UnitOfWork;
using AbsoluteCinema.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AbsoluteCinema.Services
{
    public class MovieService : IMovieService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileUpload _fileUpload;

        public MovieService(IUnitOfWork unitOfWork, IFileUpload fileUpload)
        {
            _unitOfWork = unitOfWork;
            _fileUpload = fileUpload;
        }

        public IEnumerable<Movie> GetAll()
        {
            return _unitOfWork.movieRepository.Get(
                includes: new Expression<Func<Movie, object>>[] { m => m.Category }
            );
        }
        public List<MovieVM> GetPagedMovies(string? searchTitle, int pageNumber, int pageSize, out int totalItems)
        {
            var query = _unitOfWork.movieRepository.Get(
                expression: string.IsNullOrEmpty(searchTitle) ? null : m => m.Name.Contains(searchTitle),
                includes: new Expression<Func<Movie, object>>[] { m => m.Category },
                tracked: false
            ).AsQueryable();

            totalItems = query.Count();
            var movies = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return movies.Select(m => new MovieVM
            {
                Id = m.Id,
                Title = m.Name,
                Price = m.Price,
                ExistingMainImg = m.MainImg,
                Status = m.Status,
                CategoryName = m.Category?.Name ?? string.Empty
            }).ToList();
        }

        public async Task<MovieVM?> GetMovieDetailsAsync(int id)
        {
            var movie = _unitOfWork.movieRepository.Get()
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Include(m => m.SubImgs)
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                .FirstOrDefault(m => m.Id == id);

            if (movie == null) return null;

            return new MovieVM
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
        }

        public async Task ToggleStatusAsync(int id)
        {
            var movie = _unitOfWork.movieRepository.GetOne(expression: i => i.Id == id);
            if (movie != null)
            {
                movie.Status = !movie.Status;
                _unitOfWork.movieRepository.Update(movie);
                await _unitOfWork.movieRepository.CommitAsync();
            }
        }

        public void PopulateDropdowns(MovieVM model)
        {
            model.Categories = _unitOfWork.categoryRepository.Get().Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name });
            model.Cinemas = _unitOfWork.cinemaRepository.Get().Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name });
            model.Actors = _unitOfWork.actorRepository.Get().Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name });
        }

        public async Task CreateMovieAsync(MovieVM model)
        {
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

            await _unitOfWork.movieRepository.CreateAsync(movie);
            await _unitOfWork.movieRepository.CommitAsync();

            if (model.NewSubImages != null && model.NewSubImages.Any())
            {
                foreach (var subImg in model.NewSubImages)
                {
                    string? subImgPath = _fileUpload.SaveFile(subImg, FileType.MovieSub);
                    if (!string.IsNullOrEmpty(subImgPath))
                    {
                        await _unitOfWork.subimgRepository.CreateAsync(new MovieSubImg { MovieId = movie.Id, Img = subImgPath });
                    }
                }
            }

            if (model.SelectedActorIds != null && model.SelectedActorIds.Any())
            {
                foreach (var actorId in model.SelectedActorIds)
                {
                    await _unitOfWork.movieactorRepository.CreateAsync(new MovieActor
                    {
                        MovieId = movie.Id,
                        ActorId = actorId,
                        CharacterName = "Default"
                    });
                }
            }

            await _unitOfWork.movieRepository.CommitAsync();

            char[] rows = { 'A', 'B', 'C', 'D', 'E', 'F', 'G' };
            foreach (var row in rows)
            {
                for (int i = 1; i <= 10; i++)
                {
                    var seat = new Seat
                    {
                        SeatNumber = $"{row}{i}",
                        MovieId = movie.Id,
                        IsBooked = false
                    };
                    await _unitOfWork.seatRepository.CreateAsync(seat);
                }
            }
            await _unitOfWork.seatRepository.CommitAsync();
        }

        public async Task<MovieVM?> GetMovieForUpdateAsync(int id)
        {
            var movie = _unitOfWork.movieRepository.Get()
                .Include(m => m.MovieActors)
                .Include(m => m.SubImgs)
                .FirstOrDefault(m => m.Id == id);

            if (movie == null) return null;

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
                SelectedActorIds = movie.MovieActors.Select(ma => ma.ActorId).ToList(),
                ExistingSubImages = movie.SubImgs.Select(si => new MovieSubImgVM { Id = si.Id, Img = si.Img }).ToList()
            };

            PopulateDropdowns(model);
            return model;
        }

        public async Task UpdateMovieAsync(int id, MovieVM model)
        {
            var movie = _unitOfWork.movieRepository.Get()
                .Include(m => m.MovieActors)
                .Include(m => m.SubImgs)
                .FirstOrDefault(m => m.Id == id);

            if (movie == null) return;

            string? updatedMainImg = _fileUpload.UpdateFile(model.MainImg, model.ExistingMainImg, FileType.MovieMain);

            movie.Name = model.Title;
            movie.Description = model.Description ?? string.Empty;
            movie.Price = model.Price;
            movie.DateTime = model.StartDate;
            movie.CategoryId = model.CategoryId;
            movie.CinemaId = model.CinemaId;
            movie.Status = model.Status;
            movie.MainImg = updatedMainImg ?? movie.MainImg;

            _unitOfWork.movieactorRepository.DeleteRange(movie.MovieActors);

            if (model.SelectedActorIds != null && model.SelectedActorIds.Any())
            {
                foreach (var actorId in model.SelectedActorIds)
                {
                    await _unitOfWork.movieactorRepository.CreateAsync(new MovieActor
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
                        await _unitOfWork.subimgRepository.CreateAsync(new MovieSubImg { MovieId = id, Img = subImgPath });
                    }
                }
            }

            await _unitOfWork.movieRepository.CommitAsync();
        }

        public async Task<bool> DeleteMovieAsync(int id)
        {
            var movie = _unitOfWork.movieRepository.GetOne(expression: m => m.Id == id);
            if (movie == null) return false;

            if (!string.IsNullOrEmpty(movie.MainImg))
            {
                var relativePath = movie.MainImg.TrimStart('/', '\\');
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);
                _fileUpload.DeleteFileLocally(fullPath);
            }

            var subImages = _unitOfWork.subimgRepository.Get(si => si.MovieId == id).ToList();
            foreach (var subImg in subImages)
            {
                if (!string.IsNullOrEmpty(subImg.Img))
                {
                    var subRelativePath = subImg.Img.TrimStart('/', '\\');
                    string fullSubPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", subRelativePath);
                    _fileUpload.DeleteFileLocally(fullSubPath);
                }
            }

            _unitOfWork.movieRepository.Delete(movie);
            await _unitOfWork.movieRepository.CommitAsync();
            return true;
        }

        public async Task DeleteSubImageAsync(int subImgId)
        {
            var subImg = _unitOfWork.subimgRepository.GetOne(si => si.Id == subImgId);
            if (subImg == null) return;

            if (!string.IsNullOrEmpty(subImg.Img))
            {
                var relativePath = subImg.Img.TrimStart('/', '\\');
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);
                _fileUpload.DeleteFileLocally(fullPath);
            }

            _unitOfWork.subimgRepository.Delete(subImg);
            await _unitOfWork.subimgRepository.CommitAsync();
        }

        public List<MovieVM> GetCustomerPagedMovies(string? searchTitle, int pageNumber, int pageSize, out int totalItems)
        {
            var query = _unitOfWork.movieRepository.Get(
                expression: string.IsNullOrEmpty(searchTitle) ? null : m => m.Name.Contains(searchTitle),
                includes: new Expression<Func<Movie, object>>[] { m => m.Category },
                tracked: false
            ).AsQueryable();

            totalItems = query.Count();

            var movies = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return movies.Select(m => new MovieVM
            {
                Id = m.Id,
                Title = m.Name,
                Price = m.Price,
                StartDate = m.DateTime,
                ExistingMainImg = m.MainImg,
                Status = m.Status,
                CategoryName = m.Category?.Name ?? string.Empty
            }).ToList();
        }

        public MovieVM? GetCustomerMovieDetails(int id)
        {
            var movie = _unitOfWork.movieRepository.Get()
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Include(m => m.SubImgs)
                 .Include(m => m.Seats)
                .Include(m => m.MovieActors)
                 .ThenInclude(ma => ma.Actor)
                .FirstOrDefault(m => m.Id == id);

            if (movie == null) return null;

            return new MovieVM
            {
                Id = movie.Id,
                Title = movie.Name,
                Description = movie.Description,
                Price = movie.Price,
                StartDate = movie.DateTime,
                Status = movie.Status,
                ExistingMainImg = movie.MainImg,
                CategoryName = movie.Category?.Name ?? string.Empty,
                Seats = movie.Seats,
                ExistingSubImages = movie.SubImgs.Select(si => new MovieSubImgVM { Id = si.Id, Img = si.Img }).ToList(),

                MovieActors = movie.MovieActors.Select(ma => new ActorVM
                {
                    Id = ma.Actor.Id,
                    Name = ma.Actor.Name,
                }).ToList()
            };
        }
    }
}