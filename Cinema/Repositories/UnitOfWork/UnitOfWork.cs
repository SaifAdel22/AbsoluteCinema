using AbsoluteCinema.Models;
using AbsoluteCinema.Data;
using AbsoluteCinema.Repositories.IRepositories;

namespace AbsoluteCinema.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(IRepository<Movie> movieRepository,
         IRepository<Cinema> cinemaRepository,
         IBulkRepository<MovieSubImg> subimgRepository,
         IRepository<Category> categoryRepository,
         IRepository<Actor> actorRepository,
         IBulkRepository<MovieActor> movieactorRepository,
         ApplicationDbContext context)
        {
            this.cinemaRepository = cinemaRepository;
            this.actorRepository = actorRepository;
            this.categoryRepository = categoryRepository;
            this.movieactorRepository = movieactorRepository;
            _context = context;
            this.movieRepository = movieRepository;
            this.subimgRepository = subimgRepository;
        }

        public IRepository<Cinema> cinemaRepository { get; }

        public IRepository<Category> categoryRepository { get; }


        public IRepository<Movie> movieRepository { get; }


        public IBulkRepository<MovieActor> movieactorRepository { get; }

        public IBulkRepository<MovieSubImg> subimgRepository { get; }
        public IRepository<Actor> actorRepository { get; }

        public void Dispose()
        {
             _context.Dispose();
        }
    }
}
