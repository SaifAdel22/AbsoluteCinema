using AbsoluteCinema.Data;
using AbsoluteCinema.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AbsoluteCinema.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {

        private readonly ApplicationDbContext _context;// = new();
        private readonly DbSet<T> _db;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }
        public async Task<int> CommitAsync(CancellationToken ct = default) 
        {
            try
            {
                return await _context.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return 0;
            }
        }

        public Task<bool> CreateAsync(T entity, CancellationToken ct = default)
        {
            try
            {
                _db.Add(entity);
                return Task.FromResult(true);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return Task.FromResult(false);
            }
        }

        public bool Delete(T entity)
        {
            try
            {
                _db.Remove(entity);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public IQueryable<T> Get(Expression<Func<T, bool>>? expression = null, Expression<Func<T, object>>[]? includes = null, bool tracked = true)
        {
            var enytity = _db.AsQueryable();
            if(expression != null)
            {
                enytity = enytity.Where(expression);
            }
            if(includes != null)
            {
                foreach (var include in includes)
                {
                    enytity = enytity.Include(include);
                }
            }
            if (!tracked)
            {
                enytity = enytity.AsNoTracking();
            }
            return enytity;
        }

        public T? GetOne(Expression<Func<T, bool>>? expression = null, Expression<Func<T, object>>[]? includes = null, bool tracked = true)
        {
           return Get(expression, includes, tracked).FirstOrDefault();
        }

        public bool Update(T entity)
        {
            try
            {
                _db.Update(entity);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
}
