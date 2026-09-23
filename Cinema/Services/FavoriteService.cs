namespace AbsoluteCinema.Services
{

    public class FavoriteService : IFavoriteService
    {

        public IUnitOfWork _unitOfWork;

        public FavoriteService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddToFavorites(string userId, int movieId)
        {
            var favorite = IsFavorite( userId,  movieId);
            if(favorite == true)
            {
                throw new Exception("Movie is already in favorites.");
            }
            await _unitOfWork.favoriteRepository.CreateAsync(new Favorite
            {
                ApplicationUserId = userId,
                MovieId = movieId
            });
             await _unitOfWork.favoriteRepository.CommitAsync();
        }

        public IEnumerable<Favorite> GetAllFavorites(string userId)
        {

            return _unitOfWork.favoriteRepository.Get(f => f.ApplicationUserId == userId).Include(m=>m.Movie).ToList();
        }

        public Favorite GetFavorite(string userId, int movieId)
        {

            var fav = _unitOfWork.favoriteRepository.GetOne(
         f => f.ApplicationUserId == userId && f.MovieId == movieId,
         includes: new Expression<Func<Favorite, object>>[] { f => f.Movie }
     );

            if (fav == null)
            {
                throw new Exception("Movie is not in favorites.");
            }

            return fav;
        }

      

        public bool IsFavorite(string userId, int movieId)
        {
            var result = _unitOfWork.favoriteRepository.GetOne(f => f.ApplicationUserId == userId && f.MovieId == movieId);
            if (result != null)
            {
                return true;
            }
            return false;
        }

        public async Task RemoveFromFavorites(Favorite favorite)
        {
            if (favorite != null)
            {
                _unitOfWork.favoriteRepository.Delete(favorite);
                await _unitOfWork.favoriteRepository.CommitAsync();
            }
            else
            {
                throw new Exception("Favorite not found.");
            }
        }

        
    }
}
