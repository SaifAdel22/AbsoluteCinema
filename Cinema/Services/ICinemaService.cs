using AbsoluteCinema.ViewModels;

namespace AbsoluteCinema.Services
{
    public interface ICinemaServices
    {
        List<CinemaVM> GetPagedCinemas(string? query, int pageNumber, int pageSize, out int totalItems);
        CinemaVM? GetCinemaDetails(int id);
        Task<bool> CreateCinemaAsync(CinemaVM model);
        Task<bool> UpdateCinemaAsync(int id, CinemaVM model);
        Task<bool> DeleteCinemaAsync(int id);
        bool IsCinemaNameExists(string name, int? excludeId = null);
    }
}