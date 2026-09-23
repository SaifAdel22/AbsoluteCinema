using AbsoluteCinema.Models;
using AbsoluteCinema.Repositories.UnitOfWork;
using AbsoluteCinema.ViewModels;

namespace AbsoluteCinema.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<CategoryVM> GetPagedCategories(string? query, int pageNumber, int pageSize, out int totalItems)
        {
            var categories = _unitOfWork.categoryRepository.Get();

            if (!string.IsNullOrEmpty(query))
            {
                categories = categories.Where(c => c.Name.ToLower().Contains(query.ToLower()));
            }

            totalItems = categories.Count();
            var pagedCategories = categories.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return pagedCategories.Select(c => new CategoryVM
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();
        }

        public CategoryDetailsVM? GetCategoryDetails(int id)
        {
            var category = _unitOfWork.categoryRepository.GetOne(c => c.Id == id);
            if (category == null) return null;

            return new CategoryDetailsVM
            {
                CategoryId = category.Id,
                CategoryName = category.Name,
                Movies = _unitOfWork.movieRepository.Get(m => m.CategoryId == id).Select(m => new MovieVM
                {
                    Id = m.Id,
                    Title = m.Name,
                    Description = m.Description,
                    Price = m.Price,
                    StartDate = m.DateTime,
                    ExistingMainImg = m.MainImg
                }).ToList()
            };
        }

        public CategoryVM? GetCategoryById(int id)
        {
            var category = _unitOfWork.categoryRepository.GetOne(e => e.Id == id, tracked: false);
            if (category == null) return null;

            return new CategoryVM { Id = category.Id, Name = category.Name };
        }

        public async Task CreateCategoryAsync(CategoryVM model, CancellationToken ct = default)
        {
            var category = new Category { Name = model.Name };
            await _unitOfWork.categoryRepository.CreateAsync(category, ct);
            await _unitOfWork.categoryRepository.CommitAsync(ct);
        }

        public async Task UpdateCategoryAsync(CategoryVM model, CancellationToken ct = default)
        {
            var category = new Category { Id = model.Id, Name = model.Name };
            _unitOfWork.categoryRepository.Update(category);
            await _unitOfWork.categoryRepository.CommitAsync(ct);
        }

        public async Task<bool> DeleteCategoryAsync(int id, CancellationToken ct = default)
        {
            var category = _unitOfWork.categoryRepository.GetOne(e => e.Id == id);
            if (category == null) return false;

            _unitOfWork.categoryRepository.Delete(category);
            await _unitOfWork.categoryRepository.CommitAsync(ct);
            return true;
        }
    }
}