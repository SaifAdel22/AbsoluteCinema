using AbsoluteCinema.Helper;
using AbsoluteCinema.Models;
using AbsoluteCinema.Repositories.UnitOfWork;
using AbsoluteCinema.ViewModels;
using System.Linq.Expressions;

namespace AbsoluteCinema.Services
{
    public class ActorService : IActorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileUpload _fileUpload;

        public ActorService(IUnitOfWork unitOfWork, IFileUpload fileUpload)
        {
            _unitOfWork = unitOfWork;
            _fileUpload = fileUpload;
        }

        public List<ActorDetailsVM> GetPagedActors(string? query, int pageNumber, int pageSize, out int totalItems)
        {
            var actors = _unitOfWork.actorRepository.Get(
                includes: new Expression<Func<Actor, object>>[] { a => a.MovieActors }
            );

            if (!string.IsNullOrEmpty(query))
            {
                actors = actors.Where(c => c.Name.ToLower().Contains(query.ToLower()));
            }

            totalItems = actors.Count();
            var pagedActors = actors.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return pagedActors.Select(c => new ActorDetailsVM
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
        }

        public ActorDetailsVM? GetActorDetails(int id)
        {
            var actor = _unitOfWork.actorRepository.GetOne(
                expression: c => c.Id == id,
                includes: new Expression<Func<Actor, object>>[] { c => c.MovieActors }
            );

            if (actor == null) return null;

            var actorMovies = _unitOfWork.movieRepository.Get()
                .Where(m => m.MovieActors.Any(ma => ma.ActorId == id))
                .Select(m => new MovieVM
                {
                    Id = m.Id,
                    Title = m.Name,
                    ExistingMainImg = m.MainImg,
                    Price = m.Price,
                    Description = m.Description
                }).ToList();

            return new ActorDetailsVM
            {
                ActorId = actor.Id,
                ActorName = actor.Name,
                ProfilePicture = actor.Img,
                MoviesCount = actorMovies.Count,
                Movies = actorMovies
            };
        }

        public async Task CreateActorAsync(ActorVM model)
        {
            string? imgPath = _fileUpload.SaveFile(model.ProfileImgFile, FileType.Img);
            var actor = new Actor
            {
                Name = model.Name,
                Img = imgPath ?? string.Empty
            };

            await _unitOfWork.actorRepository.CreateAsync(actor);
            await _unitOfWork.actorRepository.CommitAsync();
        }

        public async Task UpdateActorAsync(int id, ActorVM model)
        {
            var actor = _unitOfWork.actorRepository.GetOne(expression: a => a.Id == id);
            if (actor == null) return;

            string? updatedImg = _fileUpload.UpdateFile(model.ProfileImgFile, actor.Img, FileType.Img);

            actor.Name = model.Name;
            actor.Img = updatedImg ?? actor.Img;

            _unitOfWork.actorRepository.Update(actor); // أو _unitOfWork حسب اسم الفريابول عندك
            await _unitOfWork.actorRepository.CommitAsync();
        }

        public async Task DeleteActorAsync(int id)
        {
            var actor = _unitOfWork.actorRepository.Get().FirstOrDefault(i => i.Id == id);
            if (actor == null) return;

            _fileUpload.DeleteFileLocally(actor.Img);
            _unitOfWork.actorRepository.Delete(actor);
            await _unitOfWork.actorRepository.CommitAsync();
        }
    }
}