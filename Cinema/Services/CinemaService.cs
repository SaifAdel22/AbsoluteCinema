using AbsoluteCinema.Helper;
using AbsoluteCinema.Models;
using AbsoluteCinema.Repositories.UnitOfWork;
using AbsoluteCinema.ViewModels;

namespace AbsoluteCinema.Services
{
    public class CinemaService : ICinemaServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileUpload _fileUpload;

        public CinemaService(IUnitOfWork unitOfWork, IFileUpload fileUpload)
        {
            _unitOfWork = unitOfWork;
            _fileUpload = fileUpload;
        }

        public List<CinemaVM> GetPagedCinemas(string? query, int pageNumber, int pageSize, out int totalItems)
        {
            var cinemas = _unitOfWork.cinemaRepository.Get();

            if (!string.IsNullOrEmpty(query))
            {
                cinemas = cinemas.Where(c => c.Name.ToLower().Contains(query.ToLower()));
            }

            totalItems = cinemas.Count();
            var pagedCinemas = cinemas.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return pagedCinemas.Select(c => new CinemaVM
            {
                Id = c.Id,
                Name = c.Name,
                ExistingCinemaLogo = c.Img,
            }).ToList();
        }

        public CinemaVM? GetCinemaDetails(int id)
        {
            var cinema = _unitOfWork.cinemaRepository.GetOne(c => c.Id == id);
            if (cinema == null) return null;

            var cinemaMovies = _unitOfWork.movieRepository.Get(expression: m => m.CinemaId == id).Select(m => new MovieVM
            {
                Id = m.Id,
                Title = m.Name,
                ExistingMainImg = m.MainImg,
                Price = m.Price,
                Description = m.Description
            }).ToList();

         

            return new CinemaVM
            {
                Id = cinema.Id,
                Name = cinema.Name,
                ExistingCinemaLogo = cinema.Img,
                Movies = cinemaMovies
            };
        }

        public bool IsCinemaNameExists(string name, int? excludeId = null)
        {
            return _unitOfWork.cinemaRepository.GetOne(c => c.Name.ToLower() == name.ToLower() && (!excludeId.HasValue || c.Id != excludeId.Value)) != null;
        }

        public async Task<bool> CreateCinemaAsync(CinemaVM model)
        {
            string? logoPath = model.CinemaLogo != null ? _fileUpload.SaveFile(model.CinemaLogo, FileType.Img) : null;

            var cinema = new Cinema
            {
                Name = model.Name,
                Img = logoPath ?? string.Empty
            };

            await _unitOfWork.cinemaRepository.CreateAsync(cinema);
            await _unitOfWork.cinemaRepository.CommitAsync();
            return true;
        }

        public async Task<bool> UpdateCinemaAsync(int id, CinemaVM model)
        {
            var cinema = _unitOfWork.cinemaRepository.GetOne(c => c.Id == id);
            if (cinema == null) return false;

            string? logoPath = cinema.Img;
            if (model.CinemaLogo != null)
            {
                logoPath = _fileUpload.SaveFile(model.CinemaLogo, FileType.Img);
            }

            cinema.Name = model.Name;
            cinema.Img = logoPath ?? string.Empty;

            _unitOfWork.cinemaRepository.Update(cinema);
            await _unitOfWork.cinemaRepository.CommitAsync();
            return true;
        }

        public async Task<bool> DeleteCinemaAsync(int id)
        {
            var cinema = _unitOfWork.cinemaRepository.GetOne(c => c.Id == id);
            if (cinema == null) return false;

            if (!string.IsNullOrEmpty(cinema.Img))
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", cinema.Img.TrimStart('/'));
                _fileUpload.DeleteFileLocally(fullPath);
            }

            _unitOfWork.cinemaRepository.Delete(cinema);
            await _unitOfWork.cinemaRepository.CommitAsync();
            return true;
        }
    }
}