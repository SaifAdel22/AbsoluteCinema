using AbsoluteCinema.ViewModels;

namespace AbsoluteCinema.Services
{
    public interface IMovieService
    {
        public IEnumerable<Movie> GetAll();
        List<MovieVM> GetPagedMovies(string? searchTitle, int pageNumber, int pageSize, out int totalItems);
        Task<MovieVM?> GetMovieDetailsAsync(int id);
        Task ToggleStatusAsync(int id);
        Task CreateMovieAsync(MovieVM model);
        Task<MovieVM?> GetMovieForUpdateAsync(int id);
        Task UpdateMovieAsync(int id, MovieVM model);
        Task<bool> DeleteMovieAsync(int id);
        Task DeleteSubImageAsync(int subImgId);
        void PopulateDropdowns(MovieVM model);

        List<MovieVM> GetCustomerPagedMovies(string? searchTitle, int pageNumber, int pageSize, out int totalItems);
        MovieVM? GetCustomerMovieDetails(int id);
    }
}