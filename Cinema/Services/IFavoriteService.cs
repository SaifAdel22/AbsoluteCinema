namespace AbsoluteCinema.Services
{
    public interface IFavoriteService
    {
        public Task  AddToFavorites(string userId, int movieId);

        public Task  RemoveFromFavorites(Favorite favorite);
        public bool IsFavorite(string userId, int movieId);
        public IEnumerable<Favorite> GetAllFavorites(string userId);

        public Favorite GetFavorite(string userId, int movieId);

    }
}
