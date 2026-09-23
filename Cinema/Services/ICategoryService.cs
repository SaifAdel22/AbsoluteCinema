using AbsoluteCinema.ViewModels;

namespace AbsoluteCinema.Services
{
    public interface ICategoryService
    {
        List<CategoryVM> GetPagedCategories(string? query, int pageNumber, int pageSize, out int totalItems);
        CategoryDetailsVM? GetCategoryDetails(int id);
        Task CreateCategoryAsync(CategoryVM model, CancellationToken ct = default);
        Task UpdateCategoryAsync(CategoryVM model, CancellationToken ct = default);
        Task<bool> DeleteCategoryAsync(int id, CancellationToken ct = default);
        CategoryVM? GetCategoryById(int id);
    }
}